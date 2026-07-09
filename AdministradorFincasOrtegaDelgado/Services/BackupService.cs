using System.Text;
using System.Text.Json;
using AdministradorFincasOrtegaDelgado.DTOs;
using Microsoft.AspNetCore.Hosting;
using Npgsql;

namespace AdministradorFincasOrtegaDelgado.Services;

public class BackupService(
    IConfiguration        config,
    IWebHostEnvironment   env,
    ILogger<BackupService> logger) : IBackupService
{
    private readonly string _settingsPath =
        Path.Combine(env.ContentRootPath, "backup-settings.json");

    private static readonly JsonSerializerOptions JsonOpts =
        new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    // ── Settings ──────────────────────────────────────────────────────────────────

    public BackupSettingsDto GetSettings()
    {
        if (!File.Exists(_settingsPath)) return new BackupSettingsDto();
        try
        {
            var json = File.ReadAllText(_settingsPath);
            return JsonSerializer.Deserialize<BackupSettingsDto>(json, JsonOpts) ?? new BackupSettingsDto();
        }
        catch { return new BackupSettingsDto(); }
    }

    public void SaveSettings(BackupSettingsDto settings)
        => File.WriteAllText(_settingsPath, JsonSerializer.Serialize(settings, JsonOpts));

    // ── Backup creation ───────────────────────────────────────────────────────────

    public async Task<string> CreateBackupAsync()
    {
        var settings = GetSettings();
        var folder   = ResolveFolder(settings.BackupFolder);
        Directory.CreateDirectory(folder);

        var fileName = $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
        var filePath = Path.Combine(folder, fileName);

        await using var fs = File.Create(filePath);
        await WriteBackupAsync(fs);

        if (settings.MaxBackups > 0)
            PurgeOldBackups(folder, settings.MaxBackups);

        logger.LogInformation("Backup creado: {Path}", filePath);
        return filePath;
    }

    public async Task StreamBackupToAsync(Stream destination)
    {
        await WriteBackupAsync(destination);
    }

    // ── Core: genera SQL puro con Npgsql ─────────────────────────────────────────

    private async Task WriteBackupAsync(Stream output)
    {
        var connStr = config.GetConnectionString("DefaultConnection")
                      ?? throw new InvalidOperationException("No se encontró la cadena de conexión.");

        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        await using var writer = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);

        await writer.WriteLineAsync("-- Backup generado por Administrador Fincas Ortega & Delgado");
        await writer.WriteLineAsync($"-- Fecha: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        await writer.WriteLineAsync("-- Formato: SQL con INSERT statements");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("SET client_encoding = 'UTF8';");
        await writer.WriteLineAsync("SET standard_conforming_strings = on;");
        await writer.WriteLineAsync();

        // Obtener tablas del schema public (excluimos tablas internas de EF)
        var tables = await GetTablesAsync(conn);

        foreach (var table in tables)
        {
            await writer.WriteLineAsync($"-- Tabla: {table}");
            await WriteTableDataAsync(conn, writer, table);
            await writer.WriteLineAsync();
        }

        await writer.FlushAsync();
    }

    private static async Task<List<string>> GetTablesAsync(NpgsqlConnection conn)
    {
        var tables = new List<string>();
        await using var cmd = new NpgsqlCommand(
            @"SELECT table_name FROM information_schema.tables
              WHERE table_schema = 'public' AND table_type = 'BASE TABLE'
              ORDER BY table_name", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            tables.Add(reader.GetString(0));
        return tables;
    }

    private static async Task WriteTableDataAsync(NpgsqlConnection conn, StreamWriter writer, string table)
    {
        // Obtener columnas
        var columns = new List<string>();
        await using (var colCmd = new NpgsqlCommand(
            $@"SELECT column_name FROM information_schema.columns
               WHERE table_schema = 'public' AND table_name = @t
               ORDER BY ordinal_position", conn))
        {
            colCmd.Parameters.AddWithValue("t", table);
            await using var colReader = await colCmd.ExecuteReaderAsync();
            while (await colReader.ReadAsync())
                columns.Add(colReader.GetString(0));
        }

        if (columns.Count == 0) return;

        var colList = string.Join(", ", columns.Select(c => $"\"{c}\""));

        await using var dataCmd = new NpgsqlCommand($"SELECT * FROM \"{table}\"", conn);
        await using var reader  = await dataCmd.ExecuteReaderAsync();

        int rowCount = 0;
        while (await reader.ReadAsync())
        {
            var values = new List<string>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.IsDBNull(i)) { values.Add("NULL"); continue; }
                var val = reader.GetValue(i);
                values.Add(ToSqlLiteral(val));
            }
            await writer.WriteLineAsync(
                $"INSERT INTO \"{table}\" ({colList}) VALUES ({string.Join(", ", values)});");
            rowCount++;
        }

        if (rowCount == 0)
            await writer.WriteLineAsync($"-- (tabla vacía)");
    }

    private static string ToSqlLiteral(object val) => val switch
    {
        bool   b  => b ? "TRUE" : "FALSE",
        int    i  => i.ToString(),
        long   l  => l.ToString(),
        float  f  => f.ToString("G", System.Globalization.CultureInfo.InvariantCulture),
        double d  => d.ToString("G", System.Globalization.CultureInfo.InvariantCulture),
        decimal dc => dc.ToString(System.Globalization.CultureInfo.InvariantCulture),
        DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss.ffffff}'",
        DateOnly  @do => $"'{@do:yyyy-MM-dd}'",
        DateTimeOffset dto => $"'{dto:yyyy-MM-dd HH:mm:ss.ffffff zzz}'",
        _         => $"'{val.ToString()!.Replace("'", "''")}'"
    };

    // ── File management ───────────────────────────────────────────────────────────

    public IEnumerable<BackupFileDto> GetBackupFiles()
    {
        var folder = ResolveFolder(GetSettings().BackupFolder);
        if (!Directory.Exists(folder)) return [];
        return Directory.GetFiles(folder, "*.sql")
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.CreationTime)
            .Select(f => new BackupFileDto
            {
                FileName    = f.Name,
                SizeBytes   = f.Length,
                CreatedAt   = f.CreationTime,
                DisplaySize = FormatSize(f.Length),
            });
    }

    public string GetBackupFilePath(string fileName)
    {
        var safe   = Path.GetFileName(fileName);
        var folder = ResolveFolder(GetSettings().BackupFolder);
        return Path.Combine(folder, safe);
    }

    public void DeleteBackup(string fileName)
    {
        var path = GetBackupFilePath(fileName);
        if (File.Exists(path)) File.Delete(path);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────────

    private string ResolveFolder(string folder)
        => Path.IsPathRooted(folder)
            ? folder
            : Path.Combine(env.ContentRootPath, folder);

    private static void PurgeOldBackups(string folder, int maxKeep)
    {
        var toDelete = Directory
            .GetFiles(folder, "*.sql")
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.CreationTime)
            .Skip(maxKeep);
        foreach (var f in toDelete) f.Delete();
    }

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1_024         => $"{bytes} B",
        < 1_048_576     => $"{bytes / 1024.0:F1} KB",
        < 1_073_741_824 => $"{bytes / 1_048_576.0:F1} MB",
        _               => $"{bytes / 1_073_741_824.0:F1} GB",
    };
}
