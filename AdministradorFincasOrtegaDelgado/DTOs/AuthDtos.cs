namespace AdministradorFincasOrtegaDelgado.DTOs;

public record LoginRequestDto(string Email, string Password);

public record UserInfoDto(int Id, string Email, string Name, string Role);

public record LoginResponseDto(string Token, DateTime ExpiresAt, UserInfoDto User);

/// <summary>Cambio de contraseña por el propio usuario (verifica la actual)</summary>
public record ChangePasswordSelfDto(string CurrentPassword, string NewPassword);
