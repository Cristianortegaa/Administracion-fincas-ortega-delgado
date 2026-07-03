namespace AdministradorFincasOrtegaDelgado.DTOs;

public class BackupSettingsDto
{
    public string BackupFolder  { get; set; } = "backups";
    public string ScheduledTime { get; set; } = "22:00";   // HH:mm
    public bool   AutoEnabled   { get; set; } = true;
    public int    MaxBackups    { get; set; } = 30;
}

public class BackupFileDto
{
    public string   FileName    { get; set; } = "";
    public long     SizeBytes   { get; set; }
    public DateTime CreatedAt   { get; set; }
    public string   DisplaySize { get; set; } = "";
}
