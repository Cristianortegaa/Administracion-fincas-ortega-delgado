using AdministradorFincasOrtegaDelgado.DTOs;

namespace AdministradorFincasOrtegaDelgado.Services;

public interface IExpedienteService
{
    Task<IEnumerable<ExpedienteDto>> GetAllAsync(ExpedienteFilterDto? filter = null);
    Task<ExpedienteDto?> GetByIdAsync(int id);
    Task<ExpedienteDto> CreateAsync(CreateExpedienteDto dto, int? userId, string userName);
    Task<ExpedienteDto?> UpdateAsync(int id, UpdateExpedienteDto dto, int? userId, string userName);
    Task<ExpedienteDto?> CambiarTipoAsync(int id, string nuevoTipo, int? userId, string userName);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<string>> GetComunidadesAsync();
    Task<IEnumerable<string>> GetTiposIncidenciaAsync();
    Task<IEnumerable<int>> GetAñosAsync();
}
