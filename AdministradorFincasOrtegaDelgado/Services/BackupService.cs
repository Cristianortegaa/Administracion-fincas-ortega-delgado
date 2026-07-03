using System.Diagnostics;
using System.Text.Json;
using AdministradorFincasOrtegaDelgado.DTOs;
using Microsoft.AspNetCore.Hosting;

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
            return JsonSerializer.Deserialize<BackupSettingsDto>(json, JsonOpts)
                   ?? new BackupSettingsDto();
        }
        catch { return new BackupSettingsDto(); }
    }

    public void SaveSettings(BackupSettingsDto settings)
        => File.WriteAllText(_settingsPath, JsonSerializer.Serialize(settings, JsonOpts));

    // ── Backup creation ───────────────────────────────────────────────────────────

    /// <summary>Creates a backup file in the configured folder. Returns the file path.</summary>
    public async Task<string> CreateBackupAsync()
    {
        var settings = GetSettings();
        var folder   = ResolveFolder(settings.BackupFolder);
        Directory.CreateDirectory(folder);

        var fileName = $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
        var filePath = Path.Combine(folder, fileName);

        await RunPgDumpAsync(filePath);

        if (settings.MaxBackups > 0)
            PurgeOldBackups(folder, settings.MaxBackups);

        return filePath;
    }

    /// <summary>Runs pg_dump and copies output directly into <paramref name="destination"/>.</summary>
    public async Task StreamBackupToAsync(Stream destination)
    {
        var tempPath = Path.Combine(
            Path.GetTempPath(),
            $"backup_tmp_{DateTime.Now:yyyyMMddHHmmss}.sql");
        try
        {
            await RunPgDumpAsync(tempPath);
            await using var fs = File.OpenRead(tempPath);
            await fs.CopyToAsync(destination);
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

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
        // Sanitise: strip any path component to prevent traversal
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

    private async Task RunPgDumpAsync(string outputPath)
    {
        var (host, port, db, user, pass) = ParseConnectionString();

        var psi = new ProcessStartInfo
        {
            FileName               = "pg_dump",
            Arguments              = $"-h {host} -p {port} -U {user} -d {db} -F p",
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
        };
        psi.Environment["PGPASSWORD"] = pass;

        using var process = new Process { StartInfo = psi };
        process.Start();

        await using (var fileStream = File.Create(outputPath))
            await process.StandardOutput.BaseStream.CopyToAsync(fileStream);

        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            if (File.Exists(outputPath)) File.Delete(outputPath);
            throw new InvalidOperationException($"pg_dump falló (exit {process.ExitCode}): {stderr}");
        }

        logger.LogInformation("Backup creado: {Path}", outputPath);
    }

    private (string host, string port, string db, string user, string pass) ParseConnectionString()
    {
        var connStr = config.GetConnectionString("DefaultConnection") ?? "";
        var parts   = connStr
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Split('=', 2))
            .Where(p => p.Length == 2)
            .ToDictionary(p => p[0].Trim(), p => p[1].Trim(), StringComparer.OrdinalIgnoreCase);

        return (
            parts.GetValueOrDefault("Host",     "localhost"),
            parts.GetValueOrDefault("Port",     "5432"),
            parts.GetValueOrDefault("Database", "fincas_db"),
            parts.GetValueOrDefault("Username", "postgres"),
            parts.GetValueOrDefault("Password", "")
        );
    }

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

        foreach (var f in toDelete)
            f.Delete();
    }

    private static string FormatSize(long bytes) => bytes switch
    {
        < 1_024             => $"{bytes} B",
        < 1_048_576         => $"{bytes / 1024.0:F1} KB",
        < 1_073_741_824     => $"{bytes / 1_048_576.0:F1} MB",
        _                   => $"{bytes / 1_073_741_824.0:F1} GB",
    };
}
