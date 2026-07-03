namespace AdministradorFincasOrtegaDelgado.Models;

/// <summary>
/// Una entrada individual en el historial de descripciones de un expediente, siniestro o incidencia.
/// Se serializa como JSON dentro de la columna text existente.
/// </summary>
public class DescripcionEntrada
{
    public string   Texto   { get; set; } = string.Empty;
    public DateTime Fecha   { get; set; } = DateTime.UtcNow;
    public string   Usuario { get; set; } = string.Empty;
}
