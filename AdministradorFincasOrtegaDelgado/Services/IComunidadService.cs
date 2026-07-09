using AdministradorFincasOrtegaDelgado.DTOs;

namespace AdministradorFincasOrtegaDelgado.Services;

public interface IComunidadService
{
    Task<IEnumerable<ComunidadDto>> GetAllAsync();
    Task<ComunidadDto?> GetByIdAsync(int id);
    Task<ComunidadDto> CreateAsync(CreateComunidadDto dto);
    Task<ComunidadDto?> UpdateAsync(int id, UpdateComunidadDto dto);
    Task<bool> DeleteAsync(int id);
}
