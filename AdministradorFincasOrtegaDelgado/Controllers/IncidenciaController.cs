using System.Security.Claims;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorFincasOrtegaDelgado.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidenciaController : ControllerBase
{
    private readonly IIncidenciaService _service;

    public IncidenciaController(IIncidenciaService service)
    {
        _service = service;
    }

    private string GetUserName() => User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";

    /// <summary>Obtiene todas las incidencias con filtros opcionales</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IncidenciaDto>>> GetAll(
        [FromQuery] string? comunidad,
        [FromQuery] string? estado,
        [FromQuery] string? tipo,
        [FromQuery] DateOnly? fechaDesde,
        [FromQuery] DateOnly? fechaHasta,
        [FromQuery] string? search)
    {
        var filter = new IncidenciaFilterDto
        {
            Comunidad = comunidad,
            Estado = estado,
            Tipo = tipo,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Search = search
        };

        var result = await _service.GetAllAsync(filter);
        return Ok(result);
    }

    /// <summary>Obtiene una incidencia por ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<IncidenciaDto>> GetById(int id)
    {
        var incidencia = await _service.GetByIdAsync(id);
        return incidencia is null ? NotFound() : Ok(incidencia);
    }

    /// <summary>Crea una nueva incidencia</summary>
    [HttpPost]
    public async Task<ActionResult<IncidenciaDto>> Create([FromBody] CreateIncidenciaDto dto)
    {
        var created = await _service.CreateAsync(dto, GetUserName());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Actualiza una incidencia existente</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<IncidenciaDto>> Update(int id, [FromBody] UpdateIncidenciaDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto, GetUserName());
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Elimina una incidencia</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>Obtiene lista de comunidades únicas</summary>
    [HttpGet("comunidades")]
    public async Task<ActionResult<IEnumerable<string>>> GetComunidades()
    {
        var comunidades = await _service.GetComunidadesAsync();
        return Ok(comunidades);
    }

    /// <summary>Obtiene lista de tipos únicos</summary>
    [HttpGet("tipos")]
    public async Task<ActionResult<IEnumerable<string>>> GetTipos()
    {
        var tipos = await _service.GetTiposAsync();
        return Ok(tipos);
    }
}
