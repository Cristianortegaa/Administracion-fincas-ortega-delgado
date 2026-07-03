using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Models;

namespace AdministradorFincasOrtegaDelgado.Repositories;

public interface IExpedienteRepository
{
    Task<IEnumerable<Expediente>> GetAllAsync(ExpedienteFilterDto? filter = null);
    Task<Expediente?> GetByIdAsync(int id);
    Task<Expediente> CreateAsync(Expediente expediente);
    Task<Expediente> UpdateAsync(Expediente expediente);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<string>> GetComunidadesAsync();
    Task<IEnumerable<string>> GetTiposIncidenciaAsync();
    Task<IEnumerable<int>> GetAñosAsync();
}
