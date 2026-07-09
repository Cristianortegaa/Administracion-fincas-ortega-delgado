using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;
using AdministradorFincasOrtegaDelgado.Repositories;

namespace AdministradorFincasOrtegaDelgado.Services;

public class ComunidadService(IComunidadRepository repo) : IComunidadService
{
    public async Task<IEnumerable<ComunidadDto>> GetAllAsync()
    {
        var list = await repo.GetAllAsync();
        return list.Select(ToDto);
    }

    public async Task<ComunidadDto?> GetByIdAsync(int id)
    {
        var c = await repo.GetByIdAsync(id);
        return c is null ? null : ToDto(c);
    }

    public async Task<ComunidadDto> CreateAsync(CreateComunidadDto dto)
    {
        var entity = new Comunidad
        {
            Nombre          = dto.Nombre.Trim(),
            NumeroComunidad = dto.NumeroComunidad,
            CIF             = dto.CIF.Trim(),
            Direccion       = dto.Direccion.Trim(),
            CompaniaSeguros = dto.CompaniaSeguros.Trim(),
            NumeroPoliza    = dto.NumeroPoliza.Trim(),
            TelefonoSeguro  = dto.TelefonoSeguro.Trim(),
            Email           = dto.Email.Trim(),
            FechaCreacion     = DateTime.UtcNow,
            FechaModificacion = DateTime.UtcNow,
        };
        var created = await repo.CreateAsync(entity);
        return ToDto(created);
    }

    public async Task<ComunidadDto?> UpdateAsync(int id, UpdateComunidadDto dto)
    {
        var entity = await repo.GetByIdAsync(id);
        if (entity is null) return null;

        entity.Nombre          = dto.Nombre.Trim();
        entity.NumeroComunidad = dto.NumeroComunidad;
        entity.CIF             = dto.CIF.Trim();
        entity.Direccion       = dto.Direccion.Trim();
        entity.CompaniaSeguros = dto.CompaniaSeguros.Trim();
        entity.NumeroPoliza    = dto.NumeroPoliza.Trim();
        entity.TelefonoSeguro  = dto.TelefonoSeguro.Trim();
        entity.Email           = dto.Email.Trim();
        entity.FechaModificacion = DateTime.UtcNow;

        var updated = await repo.UpdateAsync(entity);
        return ToDto(updated);
    }

    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);

    private static ComunidadDto ToDto(Comunidad c) => new()
    {
        Id              = c.Id,
        Nombre          = c.Nombre,
        NumeroComunidad = c.NumeroComunidad,
        CIF             = c.CIF,
        Direccion       = c.Direccion,
        CompaniaSeguros = c.CompaniaSeguros,
        NumeroPoliza    = c.NumeroPoliza,
        TelefonoSeguro  = c.TelefonoSeguro,
        Email           = c.Email,
        FechaCreacion     = c.FechaCreacion,
        FechaModificacion = c.FechaModificacion,
    };
}
