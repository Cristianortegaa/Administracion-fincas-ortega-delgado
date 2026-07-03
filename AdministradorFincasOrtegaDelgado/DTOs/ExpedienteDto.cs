namespace AdministradorFincasOrtegaDelgado.DTOs;

// ── DescripcionEntrada DTO (compartido) ──────────────────────────────────────
public class DescripcionEntradaDto
{
    public string   Texto   { get; set; } = string.Empty;
    public DateTime Fecha   { get; set; }
    public string   Usuario { get; set; } = string.Empty;
}

// ── Respuesta ─────────────────────────────────────────────────────────────────
public class ExpedienteDto
{
    public int          Id               { get; set; }
    public string       Tipo             { get; set; } = string.Empty;  // "Incidencia" | "Siniestro"
    public string       Fecha            { get; set; } = string.Empty;  // ISO "YYYY-MM-DD"
    public string       Comunidad        { get; set; } = string.Empty;
    public int?         NumeroComunidad  { get; set; }
    public string       Ubicacion        { get; set; } = string.Empty;
    public List<DescripcionEntradaDto> Descripcion { get; set; } = new();
    public string       TipoIncidencia { get; set; } = string.Empty;
    public List<string> Reparadores    { get; set; } = new();
    public string       Observaciones  { get; set; } = string.Empty;
    public string Estado         { get; set; } = string.Empty;

    // Solo Siniestro
    public string? NumeroCDA           { get; set; }
    public string? CompaniaSeguros     { get; set; }
    public string? TelefonoCompania    { get; set; }
    public string? ReferenciaSiniestro { get; set; }

    // Auditoría
    public string   CreadoPorNombre     { get; set; } = string.Empty;
    public DateTime FechaCreacion       { get; set; }
    public string   ModificadoPorNombre { get; set; } = string.Empty;
    public DateTime FechaModificacion   { get; set; }
}

// ── Creación ──────────────────────────────────────────────────────────────────
public class CreateExpedienteDto
{
    public string       Tipo             { get; set; } = "Incidencia";
    public string       Fecha            { get; set; } = string.Empty;
    public string       Comunidad        { get; set; } = string.Empty;
    public int?         NumeroComunidad  { get; set; }
    public string       Ubicacion        { get; set; } = string.Empty;
    public string       Descripcion    { get; set; } = string.Empty;
    public string       TipoIncidencia { get; set; } = string.Empty;
    public List<string> Reparadores    { get; set; } = new();
    public string       Observaciones  { get; set; } = string.Empty;
    public string Estado         { get; set; } = "Abierto";

    // Solo Siniestro
    public string? NumeroCDA           { get; set; }
    public string? CompaniaSeguros     { get; set; }
    public string? TelefonoCompania    { get; set; }
    public string? ReferenciaSiniestro { get; set; }
}

// ── Actualización (mismos campos) ────────────────────────────────────────────
public class UpdateExpedienteDto : CreateExpedienteDto { }

// ── Cambio de tipo ────────────────────────────────────────────────────────────
public record CambiarTipoDto(string Tipo);  // "Incidencia" | "Siniestro"

// ── Filtro de búsqueda ────────────────────────────────────────────────────────
public class ExpedienteFilterDto
{
    public string? Tipo           { get; set; }
    public string? Estado         { get; set; }
    public string? Comunidad        { get; set; }
    public int?    NumeroComunidad  { get; set; }
    public string? TipoIncidencia  { get; set; }
    public string? FechaDesde     { get; set; }  // ISO "YYYY-MM-DD"
    public string? FechaHasta     { get; set; }  // ISO "YYYY-MM-DD"
    public string? Search          { get; set; }
    public int?    Anio            { get; set; }  // Filtra todos los expedientes de ese año
    public string? Empresa         { get; set; }  // Filtra por empresa reparadora (array JSON, in-memory)
    public string? CompaniaSeguros { get; set; }  // Filtra por compañía de seguros
}
