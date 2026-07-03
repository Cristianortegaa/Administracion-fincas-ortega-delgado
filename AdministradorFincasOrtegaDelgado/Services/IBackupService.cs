using AdministradorFincasOrtegaDelgado.DTOs;

namespace AdministradorFincasOrtegaDelgado.Services;

public interface IBackupService
{
    Task<string>                CreateBackupAsync();
    Task                        StreamBackupToAsync(Stream destination);
    IEnumerable<BackupFileDto>  GetBackupFiles();
    string                      GetBackupFilePath(string fileName);
    void                        DeleteBackup(string fileName);
    BackupSettingsDto           GetSettings();
    void                        SaveSettings(BackupSettingsDto settings);
}
