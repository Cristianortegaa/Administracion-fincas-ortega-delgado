using System.Security.Claims;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorFincasOrtegaDelgado.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController(IUserService userService) : ControllerBase
{
    private int? CurrentUserId =>
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

    // ── GET /api/user ─────────────────────────────────────────────────────────
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() =>
        Ok(await userService.GetAllAsync());

    // ── POST /api/user ────────────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
    {
        var (user, error) = await userService.CreateAsync(dto);
        if (error is not null)
            return BadRequest(new { message = error });

        return Ok(user);
    }

    // ── PUT /api/user/{id} ───────────────────────────────────────────────────
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var (user, error) = await userService.UpdateAsync(id, dto);
        if (error is not null)
            return BadRequest(new { message = error });
        if (user is null)
            return NotFound();
        return Ok(user);
    }

    // ── DELETE /api/user/{id} ─────────────────────────────────────────────────
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id == CurrentUserId)
            return BadRequest(new { message = "No puedes eliminar tu propia cuenta." });

        var deleted = await userService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // ── PUT /api/user/{id}/password ───────────────────────────────────────────
    [HttpPut("{id:int}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
            return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres." });

        var ok = await userService.ChangePasswordAsync(id, dto.NewPassword);
        return ok ? NoContent() : NotFound();
    }
}
