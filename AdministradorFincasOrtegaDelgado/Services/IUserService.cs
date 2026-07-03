using AdministradorFincasOrtegaDelgado.DTOs;

namespace AdministradorFincasOrtegaDelgado.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<(UserDto? user, string? error)> CreateAsync(CreateUserDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ChangePasswordAsync(int id, string newPassword);
    Task<(bool ok, string? error)> ChangePasswordSelfAsync(int userId, string currentPassword, string newPassword);
    Task<(UserDto? user, string? error)> UpdateAsync(int id, UpdateUserDto dto);
}
