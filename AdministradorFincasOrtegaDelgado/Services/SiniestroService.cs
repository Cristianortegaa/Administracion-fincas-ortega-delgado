using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Mappers;
using AdministradorFincasOrtegaDelgado.Models;
using AdministradorFincasOrtegaDelgado.Repositories;

namespace AdministradorFincasOrtegaDelgado.Services;

public class SiniestroService : ISiniestroService
{
    private readonly ISiniestroRepository _repository;

    public SiniestroService(ISiniestroRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SiniestroDto>> GetAllAsync(SiniestroFilterDto? filter = null)
    {
        var siniestros = await _repository.GetAllAsync(filter);
        return SiniestroMapper.ToDtoList(siniestros);
    }

    public async Task<SiniestroDto?> GetByIdAsync(int id)
    {
        var siniestro = await _repository.GetByIdAsync(id);
        return siniestro is null ? null : SiniestroMapper.ToDto(siniestro);
    }

    public async Task<SiniestroDto> CreateAsync(CreateSiniestroDto dto, string userName = "Sistema")
    {
        var siniestro = SiniestroMapper.ToModel(dto);

        // Add first description entry if provided
        if (!string.IsNullOrWhiteSpace(dto.DetallesSiniestro))
            siniestro.DetallesSiniestro.Add(new DescripcionEntrada
            {
                Texto   = dto.DetallesSiniestro.Trim(),
                Fecha   = DateTime.UtcNow,
                Usuario = userName
            });

        var created = await _repository.CreateAsync(siniestro);
        return SiniestroMapper.ToDto(created);
    }

    public async Task<SiniestroDto?> UpdateAsync(int id, UpdateSiniestroDto dto, string userName = "Sistema")
    {
        var siniestro = await _repository.GetByIdAsync(id);
        if (siniestro is null) return null;

        SiniestroMapper.ApplyUpdate(dto, siniestro);

        // Append new description entry if provided
        if (!string.IsNullOrWhiteSpace(dto.DetallesSiniestro))
            siniestro.DetallesSiniestro.Add(new DescripcionEntrada
            {
                Texto   = dto.DetallesSiniestro.Trim(),
                Fecha   = DateTime.UtcNow,
                Usuario = userName
            });

        var updated = await _repository.UpdateAsync(siniestro);
        return SiniestroMapper.ToDto(updated);
    }

    public async Task<bool> DeleteAsync(int id) =>
        await _repository.DeleteAsync(id);

    public async Task<IEnumerable<string>> GetComunidadesAsync() =>
        await _repository.GetComunidadesAsync();

    public async Task<IEnumerable<string>> GetCompaniasAsync() =>
        await _repository.GetCompaniasAsync();
}
