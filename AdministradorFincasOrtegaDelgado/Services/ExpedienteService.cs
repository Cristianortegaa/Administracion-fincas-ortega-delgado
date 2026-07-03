using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Mappers;
using AdministradorFincasOrtegaDelgado.Models;
using AdministradorFincasOrtegaDelgado.Repositories;

namespace AdministradorFincasOrtegaDelgado.Services;

public class ExpedienteService(IExpedienteRepository repo) : IExpedienteService
{
    public async Task<IEnumerable<ExpedienteDto>> GetAllAsync(ExpedienteFilterDto? filter = null)
    {
        var items = await repo.GetAllAsync(filter);
        return ExpedienteMapper.ToDtoList(items);
    }

    public async Task<ExpedienteDto?> GetByIdAsync(int id)
    {
        var exp = await repo.GetByIdAsync(id);
        return exp is null ? null : ExpedienteMapper.ToDto(exp);
    }

    public async Task<ExpedienteDto> CreateAsync(CreateExpedienteDto dto, int? userId, string userName)
    {
        var exp = ExpedienteMapper.ToModel(dto);
        exp.CreadoPorId      = userId;
        exp.CreadoPorNombre  = userName;
        exp.ModificadoPorId      = userId;
        exp.ModificadoPorNombre  = userName;
        exp.FechaCreacion    = DateTime.UtcNow;
        exp.FechaModificacion = DateTime.UtcNow;

        // Add first description entry if provided
        if (!string.IsNullOrWhiteSpace(dto.Descripcion))
            exp.Descripcion.Add(new Models.DescripcionEntrada
            {
                Texto   = dto.Descripcion.Trim(),
                Fecha   = DateTime.UtcNow,
                Usuario = userName
            });

        var created = await repo.CreateAsync(exp);
        return ExpedienteMapper.ToDto(created);
    }

    public async Task<ExpedienteDto?> UpdateAsync(int id, UpdateExpedienteDto dto, int? userId, string userName)
    {
        var exp = await repo.GetByIdAsync(id);
        if (exp is null) return null;

        ExpedienteMapper.ApplyUpdate(dto, exp);
        exp.ModificadoPorId      = userId;
        exp.ModificadoPorNombre  = userName;
        exp.FechaModificacion    = DateTime.UtcNow;

        // Append new description entry if provided
        if (!string.IsNullOrWhiteSpace(dto.Descripcion))
            exp.Descripcion.Add(new Models.DescripcionEntrada
            {
                Texto   = dto.Descripcion.Trim(),
                Fecha   = DateTime.UtcNow,
                Usuario = userName
            });

        var updated = await repo.UpdateAsync(exp);
        return ExpedienteMapper.ToDto(updated);
    }

    public async Task<ExpedienteDto?> CambiarTipoAsync(int id, string nuevoTipo, int? userId, string userName)
    {
        var exp = await repo.GetByIdAsync(id);
        if (exp is null) return null;

        if (!Enum.TryParse<TipoExpediente>(nuevoTipo, ignoreCase: true, out var tipo))
            return null;

        exp.Tipo = tipo;

        // Al convertir a Incidencia limpiamos campos específicos de siniestro
        if (tipo == TipoExpediente.Incidencia)
        {
            exp.NumeroCDA = null;
            exp.CompaniaSeguros = null;
            exp.TelefonoCompania = null;
            exp.ReferenciaSiniestro = null;
        }

        exp.ModificadoPorId     = userId;
        exp.ModificadoPorNombre = userName;
        exp.FechaModificacion   = DateTime.UtcNow;

        var updated = await repo.UpdateAsync(exp);
        return ExpedienteMapper.ToDto(updated);
    }

    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);

    public async Task<IEnumerable<string>> GetComunidadesAsync() =>
        await repo.GetComunidadesAsync();

    public async Task<IEnumerable<string>> GetTiposIncidenciaAsync() =>
        await repo.GetTiposIncidenciaAsync();

    public async Task<IEnumerable<int>> GetAñosAsync() =>
        await repo.GetAñosAsync();
}
