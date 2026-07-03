using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;

namespace AdministradorFincasOrtegaDelgado.Mappers;

public static class SiniestroMapper
{
    public static SiniestroDto ToDto(Siniestro siniestro) => new()
    {
        Id = siniestro.Id,
        FechaSiniestro = siniestro.FechaSiniestro,
        NumeroCDA = siniestro.NumeroCDA,
        Comunidad = siniestro.Comunidad,
        UbicacionDanio = siniestro.UbicacionDanio,
        DetallesSiniestro = siniestro.DetallesSiniestro.Select(d => new DescripcionEntradaDto
        {
            Texto   = d.Texto,
            Fecha   = d.Fecha,
            Usuario = d.Usuario
        }).ToList(),
        CompaniaSeguros = siniestro.CompaniaSeguros,
        TelefonoCompania = siniestro.TelefonoCompania,
        ReferenciaSiniestro = siniestro.ReferenciaSiniestro,
        Reparador = siniestro.Reparador,
        Estado = siniestro.Estado.ToString(),
        FechaCreacion = siniestro.FechaCreacion,
        FechaModificacion = siniestro.FechaModificacion
    };

    public static IEnumerable<SiniestroDto> ToDtoList(IEnumerable<Siniestro> siniestros) =>
        siniestros.Select(ToDto);

    // NOTE: DetallesSiniestro (new entry text) handled in Service layer.
    public static Siniestro ToModel(CreateSiniestroDto dto) => new()
    {
        FechaSiniestro = dto.FechaSiniestro,
        NumeroCDA = dto.NumeroCDA,
        Comunidad = dto.Comunidad,
        UbicacionDanio = dto.UbicacionDanio,
        DetallesSiniestro = new(),   // entries added by service
        CompaniaSeguros = dto.CompaniaSeguros,
        TelefonoCompania = dto.TelefonoCompania,
        ReferenciaSiniestro = dto.ReferenciaSiniestro,
        Reparador = dto.Reparador,
        Estado = ParseEstado(dto.Estado),
        FechaCreacion = DateTime.UtcNow,
        FechaModificacion = DateTime.UtcNow
    };

    // NOTE: DetallesSiniestro list preserved from entity; new entry appended by service.
    public static void ApplyUpdate(UpdateSiniestroDto dto, Siniestro siniestro)
    {
        siniestro.FechaSiniestro = dto.FechaSiniestro;
        siniestro.NumeroCDA = dto.NumeroCDA;
        siniestro.Comunidad = dto.Comunidad;
        siniestro.UbicacionDanio = dto.UbicacionDanio;
        // siniestro.DetallesSiniestro intentionally left untouched — service appends new entry
        siniestro.CompaniaSeguros = dto.CompaniaSeguros;
        siniestro.TelefonoCompania = dto.TelefonoCompania;
        siniestro.ReferenciaSiniestro = dto.ReferenciaSiniestro;
        siniestro.Reparador = dto.Reparador;
        siniestro.Estado = ParseEstado(dto.Estado);
        siniestro.FechaModificacion = DateTime.UtcNow;
    }

    private static EstadoSiniestro ParseEstado(string estado) =>
        estado.ToLowerInvariant() switch
        {
            "enproceso" or "en_proceso" or "proceso" => EstadoSiniestro.EnProceso,
            "finalizado" => EstadoSiniestro.Finalizado,
            _ => EstadoSiniestro.Abierto
        };
}
