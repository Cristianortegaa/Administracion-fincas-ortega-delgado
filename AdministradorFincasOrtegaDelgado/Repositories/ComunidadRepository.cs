using AdministradorFincasOrtegaDelgado.Data;
using AdministradorFincasOrtegaDelgado.Models;
using Microsoft.EntityFrameworkCore;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public class ComunidadRepository(ApplicationDbContext db) : IComunidadRepository
{
    public async Task<IEnumerable<Comunidad>> GetAllAsync() =>
        await db.Comunidades.OrderBy(c => c.NumeroComunidad).ThenBy(c => c.Nombre).ToListAsync();

    public Task<Comunidad?> GetByIdAsync(int id) =>
        db.Comunidades.FirstOrDefaultAsync(c => c.Id == id);

    public Task<Comunidad?> GetByNombreAsync(string nombre) =>
        db.Comunidades.FirstOrDefaultAsync(c => c.Nombre == nombre);

    public async Task<Comunidad> CreateAsync(Comunidad comunidad)
    {
        db.Comunidades.Add(comunidad);
        await db.SaveChangesAsync();
        return comunidad;
    }

    public async Task<Comunidad> UpdateAsync(Comunidad comunidad)
    {
        db.Comunidades.Update(comunidad);
        await db.SaveChangesAsync();
        return comunidad;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await db.Comunidades.FindAsync(id);
        if (c is null) return false;
        db.Comunidades.Remove(c);
        await db.SaveChangesAsync();
        return true;
    }
}
