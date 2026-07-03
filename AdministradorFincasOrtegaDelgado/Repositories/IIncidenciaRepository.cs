using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public interface IIncidenciaRepository
{
    Task<IEnumerable<Incidencia>> GetAllAsync(IncidenciaFilterDto? filter = null);
    Task<Incidencia?> GetByIdAsync(int id);
    Task<Incidencia> CreateAsync(Incidencia incidencia);
    Task<Incidencia> UpdateAsync(Incidencia incidencia);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<string>> GetComunidadesAsync();
    Task<IEnumerable<string>> GetTiposAsync();
}
