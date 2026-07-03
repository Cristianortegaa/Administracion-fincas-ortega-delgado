namespace AdministradorFincasOrtegaDelgado.Services;

/// <summary>
/// Runs in the background and triggers an automatic backup once per day
/// at the hour configured in BackupSettings.
/// </summary>
public class BackupHostedService(
    IBackupService            backupService,
    ILogger<BackupHostedService> logger) : BackgroundService
{
    private DateTime _lastBackupDate = DateTime.MinValue.Date;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait 30 s on startup so the app is fully initialised before first check.
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var settings = backupService.GetSettings();

                if (settings.AutoEnabled
                    && TimeOnly.TryParse(settings.ScheduledTime, out var scheduled))
                {
                    var now     = DateTime.Now;
                    var current = TimeOnly.FromDateTime(now);

                    if (current.Hour   == scheduled.Hour   &&
                        current.Minute == scheduled.Minute &&
                        _lastBackupDate < now.Date)
                    {
                        logger.LogInformation("Iniciando backup automático programado ({Time})…", settings.ScheduledTime);
                        await backupService.CreateBackupAsync();
                        _lastBackupDate = now.Date;
                        logger.LogInformation("Backup automático completado.");
                    }
                }
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error durante el backup automático.");
            }

            // Check every 60 seconds.
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
