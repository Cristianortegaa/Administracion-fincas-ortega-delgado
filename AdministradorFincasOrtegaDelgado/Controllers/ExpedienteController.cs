using System.Security.Claims;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorFincasOrtegaDelgado.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpedienteController(IExpedienteService service) : ControllerBase
{
    // ── Helpers de auditoría ──────────────────────────────────────────────────
    private int?   GetUserId()   => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
    private string GetUserName() => User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";

    // ── GET /api/expediente ───────────────────────────────────────────────────
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpedienteDto>>> GetAll(
        [FromQuery] string? tipo,
        [FromQuery] string? estado,
        [FromQuery] string? comunidad,
        [FromQuery] int?    numeroComunidad,
        [FromQuery] string? tipoIncidencia,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? search,
        [FromQuery] int?    anio,
        [FromQuery] string? empresa,
        [FromQuery] string? companiaSeguros)
    {
        var filter = new ExpedienteFilterDto
        {
            Tipo            = tipo,
            Estado          = estado,
            Comunidad       = comunidad,
            NumeroComunidad = numeroComunidad,
            TipoIncidencia  = tipoIncidencia,
            FechaDesde      = fechaDesde,
            FechaHasta      = fechaHasta,
            Search          = search,
            Anio            = anio,
            Empresa         = empresa,
            CompaniaSeguros = companiaSeguros,
        };
        return Ok(await service.GetAllAsync(filter));
    }

    // ── GET /api/expediente/{id} ──────────────────────────────────────────────
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpedienteDto>> GetById(int id)
    {
        var dto = await service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    // ── POST /api/expediente ──────────────────────────────────────────────────
    [HttpPost]
    public async Task<ActionResult<ExpedienteDto>> Create([FromBody] CreateExpedienteDto dto)
    {
        var created = await service.CreateAsync(dto, GetUserId(), GetUserName());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // ── PUT /api/expediente/{id} ──────────────────────────────────────────────
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ExpedienteDto>> Update(int id, [FromBody] UpdateExpedienteDto dto)
    {
        var updated = await service.UpdateAsync(id, dto, GetUserId(), GetUserName());
        return updated is null ? NotFound() : Ok(updated);
    }

    // ── PATCH /api/expediente/{id}/tipo ──────────────────────────────────────
    [HttpPatch("{id:int}/tipo")]
    public async Task<ActionResult<ExpedienteDto>> CambiarTipo(int id, [FromBody] CambiarTipoDto dto)
    {
        var updated = await service.CambiarTipoAsync(id, dto.Tipo, GetUserId(), GetUserName());
        return updated is null ? NotFound() : Ok(updated);
    }

    // ── DELETE /api/expediente/{id} ───────────────────────────────────────────
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // ── GET /api/expediente/comunidades ──────────────────────────────────────
    [HttpGet("comunidades")]
    public async Task<ActionResult<IEnumerable<string>>> GetComunidades() =>
        Ok(await service.GetComunidadesAsync());

    // ── GET /api/expediente/tipos-incidencia ─────────────────────────────────
    [HttpGet("tipos-incidencia")]
    public async Task<ActionResult<IEnumerable<string>>> GetTiposIncidencia() =>
        Ok(await service.GetTiposIncidenciaAsync());

    // ── GET /api/expediente/anios ─────────────────────────────────────────────
    [HttpGet("anios")]
    public async Task<ActionResult<IEnumerable<int>>> GetAnios() =>
        Ok(await service.GetAñosAsync());
}
