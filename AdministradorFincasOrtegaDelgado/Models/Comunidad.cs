namespace AdministradorFincasOrtegaDelgado.Models;

public class Comunidad
{
    public int     Id              { get; set; }
    public string  Nombre          { get; set; } = string.Empty;
    public int?    NumeroComunidad { get; set; }
    public string  CIF             { get; set; } = string.Empty;
    public string  Direccion       { get; set; } = string.Empty;
    public string  CompaniaSeguros { get; set; } = string.Empty;
    public string  NumeroPoliza    { get; set; } = string.Empty;
    public string  TelefonoSeguro  { get; set; } = string.Empty;
    public DateTime FechaCreacion     { get; set; } = DateTime.UtcNow;
    public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;
}
