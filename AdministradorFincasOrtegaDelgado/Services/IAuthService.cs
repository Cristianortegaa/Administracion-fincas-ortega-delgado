using AdministradorFincasOrtegaDelgado.DTOs;

namespace AdministradorFincasOrtegaDelgado.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
