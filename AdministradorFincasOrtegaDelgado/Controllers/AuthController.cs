using System.Security.Claims;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorFincasOrtegaDelgado.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
{
    /// <summary>Login con email y contraseña — devuelve JWT</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Email y contraseña son requeridos." });

        var result = await authService.LoginAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Credenciales incorrectas." });

        return Ok(result);
    }

    /// <summary>Verifica que el token sigue siendo válido</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var id    = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name  = User.FindFirst(ClaimTypes.Name)?.Value;
        return Ok(new { id, email, name });
    }

    /// <summary>El usuario autenticado cambia su propia contraseña (verifica la actual)</summary>
    [HttpPatch("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordSelfDto dto)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
            return Unauthorized();

        var (ok, error) = await userService.ChangePasswordSelfAsync(userId, dto.CurrentPassword, dto.NewPassword);
        return ok ? NoContent() : BadRequest(new { message = error });
    }
}
