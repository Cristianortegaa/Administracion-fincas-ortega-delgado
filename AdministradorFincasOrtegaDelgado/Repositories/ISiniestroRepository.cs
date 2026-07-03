using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public interface ISiniestroRepository
{
    Task<IEnumerable<Siniestro>> GetAllAsync(SiniestroFilterDto? filter = null);
    Task<Siniestro?> GetByIdAsync(int id);
    Task<Siniestro> CreateAsync(Siniestro siniestro);
    Task<Siniestro> UpdateAsync(Siniestro siniestro);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<string>> GetComunidadesAsync();
    Task<IEnumerable<string>> GetCompaniasAsync();
}
