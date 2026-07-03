using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;

namespace AdministradorFincasOrtegaDelgado.Mappers;

public static class IncidenciaMapper
{
    public static IncidenciaDto ToDto(Incidencia incidencia) => new()
    {
        Id = incidencia.Id,
        FechaIncidencia = incidencia.FechaIncidencia,
        Comunidad = incidencia.Comunidad,
        Ubicacion = incidencia.Ubicacion,
        Descripcion = incidencia.Descripcion.Select(d => new DescripcionEntradaDto
        {
            Texto   = d.Texto,
            Fecha   = d.Fecha,
            Usuario = d.Usuario
        }).ToList(),
        Tipo = incidencia.Tipo,
        Reparador = incidencia.Reparador,
        Observaciones = incidencia.Observaciones,
        Estado = incidencia.Estado.ToString(),
        FechaCreacion = incidencia.FechaCreacion,
        FechaModificacion = incidencia.FechaModificacion
    };

    public static IEnumerable<IncidenciaDto> ToDtoList(IEnumerable<Incidencia> incidencias) =>
        incidencias.Select(ToDto);

    // NOTE: Descripcion (new entry text) handled in Service layer.
    public static Incidencia ToModel(CreateIncidenciaDto dto) => new()
    {
        FechaIncidencia = dto.FechaIncidencia,
        Comunidad = dto.Comunidad,
        Ubicacion = dto.Ubicacion,
        Descripcion = new(),   // entries added by service
        Tipo = dto.Tipo,
        Reparador = dto.Reparador,
        Observaciones = dto.Observaciones,
        Estado = ParseEstado(dto.Estado),
        FechaCreacion = DateTime.UtcNow,
        FechaModificacion = DateTime.UtcNow
    };

    // NOTE: Descripcion list preserved from entity; new entry appended by service.
    public static void ApplyUpdate(UpdateIncidenciaDto dto, Incidencia incidencia)
    {
        incidencia.FechaIncidencia = dto.FechaIncidencia;
        incidencia.Comunidad = dto.Comunidad;
        incidencia.Ubicacion = dto.Ubicacion;
        // incidencia.Descripcion intentionally left untouched — service appends new entry
        incidencia.Tipo = dto.Tipo;
        incidencia.Reparador = dto.Reparador;
        incidencia.Observaciones = dto.Observaciones;
        incidencia.Estado = ParseEstado(dto.Estado);
        incidencia.FechaModificacion = DateTime.UtcNow;
    }

    private static EstadoIncidencia ParseEstado(string estado) =>
        estado.ToLowerInvariant() switch
        {
            "enproceso" or "en_proceso" or "proceso" => EstadoIncidencia.EnProceso,
            "resuelta" or "finalizada" or "finalizado" => EstadoIncidencia.Resuelta,
            _ => EstadoIncidencia.Abierta
        };
}
