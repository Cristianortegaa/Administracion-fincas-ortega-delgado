using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;

namespace AdministradorFincasOrtegaDelgado.Mappers;

public static class ExpedienteMapper
{
    // ── Entity → DTO ──────────────────────────────────────────────────────────
    public static ExpedienteDto ToDto(Expediente e) => new()
    {
        Id              = e.Id,
        Tipo            = e.Tipo.ToString(),
        Fecha           = e.Fecha.ToString("yyyy-MM-dd"),
        Comunidad       = e.Comunidad,
        NumeroComunidad = e.NumeroComunidad,
        Ubicacion       = e.Ubicacion,
        Descripcion     = e.Descripcion.Select(d => new DescripcionEntradaDto
        {
            Texto   = d.Texto,
            Fecha   = d.Fecha,
            Usuario = d.Usuario
        }).ToList(),
        TipoIncidencia  = e.TipoIncidencia,
        Reparadores     = e.Reparadores,
        Observaciones   = e.Observaciones,
        Estado          = e.Estado.ToString(),

        NumeroCDA           = e.NumeroCDA,
        CompaniaSeguros     = e.CompaniaSeguros,
        NumeroPoliza        = e.NumeroPoliza,
        TelefonoCompania    = e.TelefonoCompania,
        ReferenciaSiniestro = e.ReferenciaSiniestro,

        CreadoPorNombre     = e.CreadoPorNombre,
        FechaCreacion       = e.FechaCreacion,
        ModificadoPorNombre = e.ModificadoPorNombre,
        FechaModificacion   = e.FechaModificacion,
    };

    public static IEnumerable<ExpedienteDto> ToDtoList(IEnumerable<Expediente> list) =>
        list.Select(ToDto);

    // ── CreateDto → Entity ────────────────────────────────────────────────────
    // NOTE: Descripcion (new entry text) is handled in the Service layer.
    public static Expediente ToModel(CreateExpedienteDto dto) => new()
    {
        Tipo            = ParseTipo(dto.Tipo),
        Fecha           = DateOnly.TryParse(dto.Fecha, out var f) ? f : DateOnly.FromDateTime(DateTime.Today),
        Comunidad       = dto.Comunidad,
        NumeroComunidad = dto.NumeroComunidad,
        Ubicacion       = dto.Ubicacion,
        Descripcion     = new(),   // entries added by service
        TipoIncidencia  = dto.TipoIncidencia,
        Reparadores     = dto.Reparadores ?? new(),
        Observaciones   = dto.Observaciones,
        Estado          = ParseEstado(dto.Estado),

        NumeroCDA           = dto.NumeroCDA,
        CompaniaSeguros     = dto.CompaniaSeguros,
        NumeroPoliza        = dto.NumeroPoliza,
        TelefonoCompania    = dto.TelefonoCompania,
        ReferenciaSiniestro = dto.ReferenciaSiniestro,
    };

    // ── UpdateDto → Entity (in-place) ─────────────────────────────────────────
    // NOTE: Descripcion list is preserved from the entity; new entry appended by service.
    public static void ApplyUpdate(UpdateExpedienteDto dto, Expediente e)
    {
        e.Tipo            = ParseTipo(dto.Tipo);
        e.Fecha           = DateOnly.TryParse(dto.Fecha, out var f) ? f : e.Fecha;
        e.Comunidad       = dto.Comunidad;
        e.NumeroComunidad = dto.NumeroComunidad;
        e.Ubicacion       = dto.Ubicacion;
        // e.Descripcion intentionally left untouched — service appends new entry
        e.TipoIncidencia  = dto.TipoIncidencia;
        e.Reparadores     = dto.Reparadores ?? new();
        e.Observaciones   = dto.Observaciones;
        e.Estado          = ParseEstado(dto.Estado);

        e.NumeroCDA           = dto.NumeroCDA;
        e.CompaniaSeguros     = dto.CompaniaSeguros;
        e.NumeroPoliza        = dto.NumeroPoliza;
        e.TelefonoCompania    = dto.TelefonoCompania;
        e.ReferenciaSiniestro = dto.ReferenciaSiniestro;

        // Limpia campos de siniestro si se convierte a incidencia
        if (e.Tipo == TipoExpediente.Incidencia)
        {
            e.NumeroCDA = null;
            e.CompaniaSeguros = null;
            e.NumeroPoliza = null;
            e.TelefonoCompania = null;
            e.ReferenciaSiniestro = null;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static TipoExpediente ParseTipo(string s) =>
        Enum.TryParse<TipoExpediente>(s, ignoreCase: true, out var t) ? t : TipoExpediente.Incidencia;

    private static EstadoExpediente ParseEstado(string s) =>
        Enum.TryParse<EstadoExpediente>(s, ignoreCase: true, out var e) ? e : EstadoExpediente.Abierto;
}
