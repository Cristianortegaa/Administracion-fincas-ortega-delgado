namespace AdministradorFincasOrtegaDelgado.Models;

public enum TipoExpediente   { Incidencia, Siniestro }
public enum EstadoExpediente  { Abierto, Cerrado }

/// <summary>
/// Entidad unificada que representa tanto incidencias como siniestros.
/// Tipo = Incidencia → gestión interna sin parte al seguro.
/// Tipo = Siniestro  → parte tramitado con compañía aseguradora.
/// </summary>
public class Expediente
{
    public int Id { get; set; }

    // ── Tipo (discriminador) ──────────────────────────────
    public TipoExpediente Tipo { get; set; } = TipoExpediente.Incidencia;

    // ── Campos comunes ────────────────────────────────────
    public DateOnly Fecha { get; set; }
    public string Comunidad        { get; set; } = string.Empty;
    public int?   NumeroComunidad { get; set; }
    public string Ubicacion        { get; set; } = string.Empty;
    public List<DescripcionEntrada> Descripcion { get; set; } = new();
    public string TipoIncidencia   { get; set; } = string.Empty; // Fontanería, Eléctrico…
    public List<string> Reparadores { get; set; } = new();       // Puede haber varias empresas
    public string Observaciones    { get; set; } = string.Empty;
    public EstadoExpediente Estado { get; set; } = EstadoExpediente.Abierto;

    // ── Solo cuando Tipo = Siniestro ──────────────────────
    public string? NumeroCDA          { get; set; }
    public string? CompaniaSeguros    { get; set; }
    public string? TelefonoCompania   { get; set; }
    public string? ReferenciaSiniestro { get; set; }

    // ── Auditoría ─────────────────────────────────────────
    public int?   CreadoPorId      { get; set; }
    public string CreadoPorNombre  { get; set; } = string.Empty;
    public DateTime FechaCreacion  { get; set; } = DateTime.UtcNow;

    public int?   ModificadoPorId      { get; set; }
    public string ModificadoPorNombre  { get; set; } = string.Empty;
    public DateTime FechaModificacion  { get; set; } = DateTime.UtcNow;
}
