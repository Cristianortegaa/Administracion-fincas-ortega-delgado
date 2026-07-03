namespace AdministradorFincasOrtegaDelgado.DTOs;

// ── Respuesta ────────────────────────────────────────────────────────────────
public class UserDto
{
    public int      Id        { get; set; }
    public string   Name      { get; set; } = string.Empty;
    public string   Email     { get; set; } = string.Empty;
    public string   Role      { get; set; } = "Worker";
    public DateTime CreatedAt { get; set; }
}

// ── Creación ─────────────────────────────────────────────────────────────────
public class CreateUserDto
{
    public string Name     { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// ── Edición de usuario ────────────────────────────────────────────────────────
public class UpdateUserDto
{
    public string Name        { get; set; } = string.Empty;
    /// <summary>Si viene vacío no se cambia la contraseña</summary>
    public string NewPassword { get; set; } = string.Empty;
}

// ── Cambio de contraseña ──────────────────────────────────────────────────────
public class ChangePasswordDto
{
    public string NewPassword { get; set; } = string.Empty;
}
