using AdministradorFincasOrtegaDelgado.Models;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public interface IComunidadRepository
{
    Task<IEnumerable<Comunidad>> GetAllAsync();
    Task<Comunidad?> GetByIdAsync(int id);
    Task<Comunidad?> GetByNombreAsync(string nombre);
    Task<Comunidad> CreateAsync(Comunidad comunidad);
    Task<Comunidad> UpdateAsync(Comunidad comunidad);
    Task<bool> DeleteAsync(int id);
}
