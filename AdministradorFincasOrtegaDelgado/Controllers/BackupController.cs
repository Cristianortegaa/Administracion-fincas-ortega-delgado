using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorFincasOrtegaDelgado.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class BackupController(IBackupService backupService) : ControllerBase
{
    // ── POST /api/backup/now ── descarga inmediata ────────────────────────────────
    [HttpPost("now")]
    public async Task<IActionResult> DownloadNow()
    {
        try
        {
            var ms = new MemoryStream();
            await backupService.StreamBackupToAsync(ms);
            ms.Position = 0;

            var fileName = $"backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.sql";
            return File(ms, "application/octet-stream", fileName);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al crear la copia de seguridad: {ex.Message}" });
        }
    }

    // ── POST /api/backup/restore ── restaura desde archivo .sql ─────────────────
    [HttpPost("restore")]
    [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB máximo
    public async Task<IActionResult> Restore(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "No se ha proporcionado ningún archivo." });

        if (!file.FileName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "El archivo debe ser un .sql generado por esta aplicación." });

        try
        {
            await using var stream = file.OpenReadStream();
            await backupService.RestoreFromSqlAsync(stream);
            return Ok(new { message = "Base de datos restaurada correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al restaurar: {ex.Message}" });
        }
    }

    // ── POST /api/backup/save ── crea y guarda en servidor ───────────────────────
    [HttpPost("save")]
    public async Task<IActionResult> SaveToServer()
    {
        try
        {
            var path = await backupService.CreateBackupAsync();
            return Ok(new { message = "Copia guardada en el servidor.", path });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Error al guardar la copia: {ex.Message}" });
        }
    }

    // ── GET /api/backup/files ── lista de backups ────────────────────────────────
    [HttpGet("files")]
    public ActionResult<IEnumerable<BackupFileDto>> GetFiles()
        => Ok(backupService.GetBackupFiles());

    // ── GET /api/backup/files/{fileName} ── descargar backup ─────────────────────
    [HttpGet("files/{fileName}")]
    public IActionResult DownloadFile(string fileName)
    {
        if (IsInvalidFileName(fileName))
            return BadRequest(new { message = "Nombre de archivo no válido." });

        var path = backupService.GetBackupFilePath(fileName);
        if (!System.IO.File.Exists(path))
            return NotFound(new { message = "Archivo no encontrado." });

        var stream = System.IO.File.OpenRead(path);
        return File(stream, "application/octet-stream", fileName);
    }

    // ── DELETE /api/backup/files/{fileName} ── eliminar backup ───────────────────
    [HttpDelete("files/{fileName}")]
    public IActionResult DeleteFile(string fileName)
    {
        if (IsInvalidFileName(fileName))
            return BadRequest(new { message = "Nombre de archivo no válido." });

        backupService.DeleteBackup(fileName);
        return NoContent();
    }

    // ── GET /api/backup/settings ──────────────────────────────────────────────────
    [HttpGet("settings")]
    public ActionResult<BackupSettingsDto> GetSettings()
        => Ok(backupService.GetSettings());

    // ── PUT /api/backup/settings ──────────────────────────────────────────────────
    [HttpPut("settings")]
    public IActionResult SaveSettings([FromBody] BackupSettingsDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.BackupFolder))
            return BadRequest(new { message = "La carpeta de backup es obligatoria." });
        if (!TimeOnly.TryParse(dto.ScheduledTime, out _))
            return BadRequest(new { message = "Formato de hora no válido. Usa HH:mm." });

        backupService.SaveSettings(dto);
        return NoContent();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────────
    private static bool IsInvalidFileName(string name)
        => name.Contains('/') || name.Contains('\\') || name.Contains("..");
}
