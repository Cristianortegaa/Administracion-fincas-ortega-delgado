namespace AdministradorFincasOrtegaDelgado.Models;

public class Incidencia
{
    public int Id { get; set; }
    public DateOnly FechaIncidencia { get; set; }
    public string Comunidad { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public List<DescripcionEntrada> Descripcion { get; set; } = new();
    public string Tipo { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public EstadoIncidencia Estado { get; set; } = EstadoIncidencia.Abierta;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
}

public enum EstadoIncidencia
{
    Abierta,
    EnProceso,
    Resuelta
}
