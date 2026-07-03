namespace AdministradorFincasOrtegaDelgado.Models;

public class Siniestro
{
    public int Id { get; set; }
    public DateOnly FechaSiniestro { get; set; }
    public string NumeroCDA { get; set; } = string.Empty;
    public string Comunidad { get; set; } = string.Empty;
    public string UbicacionDanio { get; set; } = string.Empty;
    public List<DescripcionEntrada> DetallesSiniestro { get; set; } = new();
    public string CompaniaSeguros { get; set; } = string.Empty;
    public string TelefonoCompania { get; set; } = string.Empty;
    public string ReferenciaSiniestro { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public EstadoSiniestro Estado { get; set; } = EstadoSiniestro.Abierto;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
}

public enum EstadoSiniestro
{
    Abierto,
    EnProceso,
    Finalizado
}
