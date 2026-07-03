using AdministradorFincasOrtegaDelgado.Data;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public class IncidenciaRepository : IIncidenciaRepository
{
    private readonly ApplicationDbContext _context;

    public IncidenciaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Incidencia>> GetAllAsync(IncidenciaFilterDto? filter = null)
    {
        var query = _context.Incidencias.AsQueryable();

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Comunidad))
                query = query.Where(i => i.Comunidad.Contains(filter.Comunidad));

            if (!string.IsNullOrWhiteSpace(filter.Estado) &&
                Enum.TryParse<EstadoIncidencia>(filter.Estado, ignoreCase: true, out var estado))
                query = query.Where(i => i.Estado == estado);

            if (!string.IsNullOrWhiteSpace(filter.Tipo))
                query = query.Where(i => i.Tipo.Contains(filter.Tipo));

            if (filter.FechaDesde.HasValue)
                query = query.Where(i => i.FechaIncidencia >= filter.FechaDesde.Value);

            if (filter.FechaHasta.HasValue)
                query = query.Where(i => i.FechaIncidencia <= filter.FechaHasta.Value);

        }

        // Load to memory first; Descripcion is a JSON list and cannot be filtered in SQL
        var list = await query.OrderByDescending(i => i.FechaIncidencia).ToListAsync();

        if (filter is not null && !string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            list = list.Where(i =>
                i.Comunidad.ToLower().Contains(search)                             ||
                i.Ubicacion.ToLower().Contains(search)                             ||
                i.Descripcion.Any(d => d.Texto.ToLower().Contains(search))         ||
                i.Tipo.ToLower().Contains(search)                                  ||
                i.Reparador.ToLower().Contains(search)                             ||
                i.Observaciones.ToLower().Contains(search)).ToList();
        }

        return list;
    }

    public async Task<Incidencia?> GetByIdAsync(int id) =>
        await _context.Incidencias.FindAsync(id);

    public async Task<Incidencia> CreateAsync(Incidencia incidencia)
    {
        _context.Incidencias.Add(incidencia);
        await _context.SaveChangesAsync();
        return incidencia;
    }

    public async Task<Incidencia> UpdateAsync(Incidencia incidencia)
    {
        _context.Incidencias.Update(incidencia);
        await _context.SaveChangesAsync();
        return incidencia;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var incidencia = await _context.Incidencias.FindAsync(id);
        if (incidencia is null) return false;
        _context.Incidencias.Remove(incidencia);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetComunidadesAsync() =>
        await _context.Incidencias
            .Select(i => i.Comunidad)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    public async Task<IEnumerable<string>> GetTiposAsync() =>
        await _context.Incidencias
            .Where(i => !string.IsNullOrEmpty(i.Tipo))
            .Select(i => i.Tipo)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
}
