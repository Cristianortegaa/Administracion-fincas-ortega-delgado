using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Mappers;
using AdministradorFincasOrtegaDelgado.Models;
using AdministradorFincasOrtegaDelgado.Repositories;

namespace AdministradorFincasOrtegaDelgado.Services;

public class IncidenciaService : IIncidenciaService
{
    private readonly IIncidenciaRepository _repository;

    public IncidenciaService(IIncidenciaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<IncidenciaDto>> GetAllAsync(IncidenciaFilterDto? filter = null)
    {
        var incidencias = await _repository.GetAllAsync(filter);
        return IncidenciaMapper.ToDtoList(incidencias);
    }

    public async Task<IncidenciaDto?> GetByIdAsync(int id)
    {
        var incidencia = await _repository.GetByIdAsync(id);
        return incidencia is null ? null : IncidenciaMapper.ToDto(incidencia);
    }

    public async Task<IncidenciaDto> CreateAsync(CreateIncidenciaDto dto, string userName = "Sistema")
    {
        var incidencia = IncidenciaMapper.ToModel(dto);

        // Add first description entry if provided
        if (!string.IsNullOrWhiteSpace(dto.Descripcion))
            incidencia.Descripcion.Add(new DescripcionEntrada
            {
                Texto   = dto.Descripcion.Trim(),
                Fecha   = DateTime.UtcNow,
                Usuario = userName
            });

        var created = await _repository.CreateAsync(incidencia);
        return IncidenciaMapper.ToDto(created);
    }

    public async Task<IncidenciaDto?> UpdateAsync(int id, UpdateIncidenciaDto dto, string userName = "Sistema")
    {
        var incidencia = await _repository.GetByIdAsync(id);
        if (incidencia is null) return null;

        IncidenciaMapper.ApplyUpdate(dto, incidencia);

        // Append new description entry if provided
        if (!string.IsNullOrWhiteSpace(dto.Descripcion))
            incidencia.Descripcion.Add(new DescripcionEntrada
            {
                Texto   = dto.Descripcion.Trim(),
                Fecha   = DateTime.UtcNow,
                Usuario = userName
            });

        var updated = await _repository.UpdateAsync(incidencia);
        return IncidenciaMapper.ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id) =>
        await _repository.DeleteAsync(id);

    public async Task<IEnumerable<string>> GetComunidadesAsync() =>
        await _repository.GetComunidadesAsync();

    public async Task<IEnumerable<string>> GetTiposAsync() =>
        await _repository.GetTiposAsync();
}
