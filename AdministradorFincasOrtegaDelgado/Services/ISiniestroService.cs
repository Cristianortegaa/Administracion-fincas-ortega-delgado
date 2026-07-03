using AdministradorFincasOrtegaDelgado.DTOs;

namespace AdministradorFincasOrtegaDelgado.Services;

public interface ISiniestroService
{
    Task<IEnumerable<SiniestroDto>> GetAllAsync(SiniestroFilterDto? filter = null);
    Task<SiniestroDto?> GetByIdAsync(int id);
    Task<SiniestroDto> CreateAsync(CreateSiniestroDto dto, string userName = "Sistema");
    Task<SiniestroDto?> UpdateAsync(int id, UpdateSiniestroDto dto, string userName = "Sistema");
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<string>> GetComunidadesAsync();
    Task<IEnumerable<string>> GetCompaniasAsync();
}
