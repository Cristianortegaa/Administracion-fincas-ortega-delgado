using AdministradorFincasOrtegaDelgado.Data;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public class SiniestroRepository : ISiniestroRepository
{
    private readonly ApplicationDbContext _context;

    public SiniestroRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Siniestro>> GetAllAsync(SiniestroFilterDto? filter = null)
    {
        var query = _context.Siniestros.AsQueryable();

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Comunidad))
                query = query.Where(s => s.Comunidad.Contains(filter.Comunidad));

            if (!string.IsNullOrWhiteSpace(filter.Estado) &&
                Enum.TryParse<EstadoSiniestro>(filter.Estado, ignoreCase: true, out var estado))
                query = query.Where(s => s.Estado == estado);

            if (!string.IsNullOrWhiteSpace(filter.CompaniaSeguros))
                query = query.Where(s => s.CompaniaSeguros.Contains(filter.CompaniaSeguros));

            if (filter.FechaDesde.HasValue)
                query = query.Where(s => s.FechaSiniestro >= filter.FechaDesde.Value);

            if (filter.FechaHasta.HasValue)
                query = query.Where(s => s.FechaSiniestro <= filter.FechaHasta.Value);

        }

        // Load to memory first; DetallesSiniestro is a JSON list and cannot be filtered in SQL
        var list = await query.OrderByDescending(s => s.FechaSiniestro).ToListAsync();

        if (filter is not null && !string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.ToLower();
            list = list.Where(s =>
                s.NumeroCDA.ToLower().Contains(search)                               ||
                s.Comunidad.ToLower().Contains(search)                               ||
                s.UbicacionDanio.ToLower().Contains(search)                          ||
                s.DetallesSiniestro.Any(d => d.Texto.ToLower().Contains(search))     ||
                s.ReferenciaSiniestro.ToLower().Contains(search)                     ||
                s.Reparador.ToLower().Contains(search)).ToList();
        }

        return list;
    }

    public async Task<Siniestro?> GetByIdAsync(int id) =>
        await _context.Siniestros.FindAsync(id);

    public async Task<Siniestro> CreateAsync(Siniestro siniestro)
    {
        _context.Siniestros.Add(siniestro);
        await _context.SaveChangesAsync();
        return siniestro;
    }

    public async Task<Siniestro> UpdateAsync(Siniestro siniestro)
    {
        _context.Siniestros.Update(siniestro);
        await _context.SaveChangesAsync();
        return siniestro;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var siniestro = await _context.Siniestros.FindAsync(id);
        if (siniestro is null) return false;
        _context.Siniestros.Remove(siniestro);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetComunidadesAsync() =>
        await _context.Siniestros
            .Select(s => s.Comunidad)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    public async Task<IEnumerable<string>> GetCompaniasAsync() =>
        await _context.Siniestros
            .Select(s => s.CompaniaSeguros)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
}
