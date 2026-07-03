using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Helpers;
using AdministradorFincasOrtegaDelgado.Models;
using AdministradorFincasOrtegaDelgado.Repositories;

namespace AdministradorFincasOrtegaDelgado.Services;

public class UserService(IUserRepository repo) : IUserService
{
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await repo.GetAllAsync();
        return users.Select(u => ToDto(u));
    }

    public async Task<(UserDto? user, string? error)> CreateAsync(CreateUserDto dto)
    {
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (null, "El nombre es obligatorio.");

        if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains('@'))
            return (null, "El email no es válido.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return (null, "La contraseña debe tener al menos 6 caracteres.");

        // Email único
        var existing = await repo.GetByEmailAsync(dto.Email);
        if (existing is not null)
            return (null, "Ya existe una cuenta con ese email.");

        var user = new User
        {
            Name         = dto.Name.Trim(),
            Email        = dto.Email.Trim().ToLower(),
            PasswordHash = PasswordHasher.Hash(dto.Password),
            CreatedAt    = DateTime.UtcNow,
            Role         = "Worker",
        };

        await repo.AddAsync(user);
        await repo.SaveAsync();

        return (ToDto(user), null);
    }

    public async Task<(UserDto? user, string? error)> UpdateAsync(int id, UpdateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return (null, "El nombre es obligatorio.");

        if (!string.IsNullOrWhiteSpace(dto.NewPassword) && dto.NewPassword.Length < 6)
            return (null, "La contraseña debe tener al menos 6 caracteres.");

        var user = await repo.GetByIdAsync(id);
        if (user is null) return (null, "Usuario no encontrado.");

        user.Name = dto.Name.Trim();

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);

        await repo.SaveAsync();
        return (ToDto(user), null);
    }

    public Task<bool> DeleteAsync(int id) => repo.DeleteAsync(id);

    public async Task<(bool ok, string? error)> ChangePasswordSelfAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await repo.GetByIdAsync(userId);
        if (user is null) return (false, "Usuario no encontrado.");

        if (!PasswordHasher.Verify(currentPassword, user.PasswordHash))
            return (false, "La contraseña actual no es correcta.");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            return (false, "La nueva contraseña debe tener al menos 6 caracteres.");

        user.PasswordHash = PasswordHasher.Hash(newPassword);
        await repo.SaveAsync();
        return (true, null);
    }

    public async Task<bool> ChangePasswordAsync(int id, string newPassword)
    {
        var user = await repo.GetByIdAsync(id);
        if (user is null) return false;

        user.PasswordHash = PasswordHasher.Hash(newPassword);
        await repo.SaveAsync();
        return true;
    }

    private static UserDto ToDto(User u) => new()
    {
        Id        = u.Id,
        Name      = u.Name,
        Email     = u.Email,
        Role      = u.Role,
        CreatedAt = u.CreatedAt,
    };
}
