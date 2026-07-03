using AdministradorFincasOrtegaDelgado.Data;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public class ExpedienteRepository(ApplicationDbContext db) : IExpedienteRepository
{
    public async Task<IEnumerable<Expediente>> GetAllAsync(ExpedienteFilterDto? filter = null)
    {
        var q = db.Expedientes.AsQueryable();

        if (filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Tipo) &&
                Enum.TryParse<TipoExpediente>(filter.Tipo, ignoreCase: true, out var tipo))
                q = q.Where(e => e.Tipo == tipo);

            if (!string.IsNullOrWhiteSpace(filter.Estado) &&
                Enum.TryParse<EstadoExpediente>(filter.Estado, ignoreCase: true, out var estado))
                q = q.Where(e => e.Estado == estado);

            if (!string.IsNullOrWhiteSpace(filter.Comunidad))
                q = q.Where(e => e.Comunidad.Contains(filter.Comunidad));

            if (filter.NumeroComunidad.HasValue)
                q = q.Where(e => e.NumeroComunidad == filter.NumeroComunidad.Value);

            if (!string.IsNullOrWhiteSpace(filter.TipoIncidencia))
                q = q.Where(e => e.TipoIncidencia == filter.TipoIncidencia);

            if (filter.Anio.HasValue)
                q = q.Where(e => e.Fecha.Year == filter.Anio.Value);

            if (DateOnly.TryParse(filter.FechaDesde, out var desde))
                q = q.Where(e => e.Fecha >= desde);

            if (DateOnly.TryParse(filter.FechaHasta, out var hasta))
                q = q.Where(e => e.Fecha <= hasta);

            if (!string.IsNullOrWhiteSpace(filter.CompaniaSeguros))
                q = q.Where(e => e.CompaniaSeguros != null && e.CompaniaSeguros.Contains(filter.CompaniaSeguros));
        }

        // Reparadores es JSON → no se puede filtrar en SQL; traemos y filtramos en memoria
        var list = await q.OrderByDescending(e => e.FechaCreacion).ToListAsync();

        if (filter is not null && !string.IsNullOrWhiteSpace(filter.Empresa))
            list = list.Where(e => e.Reparadores.Any(r => r == filter.Empresa)).ToList();

        if (filter is not null && !string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            list = list.Where(e =>
                e.Comunidad.ToLower().Contains(s)                                                            ||
                e.Descripcion.Any(d => d.Texto.ToLower().Contains(s))                                       ||
                e.TipoIncidencia.ToLower().Contains(s)                                                       ||
                e.Reparadores.Any(r => r.ToLower().Contains(s))                                              ||
                (e.NumeroCDA != null && e.NumeroCDA.ToLower().Contains(s))                                   ||
                (e.ReferenciaSiniestro != null && e.ReferenciaSiniestro.ToLower().Contains(s)))
            .ToList();
        }

        return list;
    }

    public Task<Expediente?> GetByIdAsync(int id) =>
        db.Expedientes.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Expediente> CreateAsync(Expediente exp)
    {
        db.Expedientes.Add(exp);
        await db.SaveChangesAsync();
        return exp;
    }

    public async Task<Expediente> UpdateAsync(Expediente exp)
    {
        db.Expedientes.Update(exp);
        await db.SaveChangesAsync();
        return exp;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var exp = await db.Expedientes.FindAsync(id);
        if (exp is null) return false;
        db.Expedientes.Remove(exp);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetComunidadesAsync() =>
        await db.Expedientes
            .Select(e => e.Comunidad)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

    public async Task<IEnumerable<string>> GetTiposIncidenciaAsync() =>
        await db.Expedientes
            .Select(e => e.TipoIncidencia)
            .Where(t => !string.IsNullOrEmpty(t))
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();

    public async Task<IEnumerable<int>> GetAñosAsync() =>
        await db.Expedientes
            .Select(e => e.Fecha.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();
}
