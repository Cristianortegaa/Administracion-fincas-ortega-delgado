using AdministradorFincasOrtegaDelgado.DTOs;

namespace AdministradorFincasOrtegaDelgado.Services;

public interface IIncidenciaService
{
    Task<IEnumerable<IncidenciaDto>> GetAllAsync(IncidenciaFilterDto? filter = null);
    Task<IncidenciaDto?> GetByIdAsync(int id);
    Task<IncidenciaDto> CreateAsync(CreateIncidenciaDto dto, string userName = "Sistema");
    Task<IncidenciaDto?> UpdateAsync(int id, UpdateIncidenciaDto dto, string userName = "Sistema");
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<string>> GetComunidadesAsync();
    Task<IEnumerable<string>> GetTiposAsync();
}
