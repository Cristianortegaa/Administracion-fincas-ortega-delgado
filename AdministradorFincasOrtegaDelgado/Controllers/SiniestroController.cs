using System.Security.Claims;
using AdministradorFincasOrtegaDelgado.DTOs;
using AdministradorFincasOrtegaDelgado.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministradorFincasOrtegaDelgado.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SiniestroController : ControllerBase
{
    private readonly ISiniestroService _service;

    public SiniestroController(ISiniestroService service)
    {
        _service = service;
    }

    private string GetUserName() => User.FindFirst(ClaimTypes.Name)?.Value ?? "Sistema";

    /// <summary>Obtiene todos los siniestros con filtros opcionales</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SiniestroDto>>> GetAll(
        [FromQuery] string? comunidad,
        [FromQuery] string? estado,
        [FromQuery] string? companiaSeguros,
        [FromQuery] DateOnly? fechaDesde,
        [FromQuery] DateOnly? fechaHasta,
        [FromQuery] string? search)
    {
        var filter = new SiniestroFilterDto
        {
            Comunidad = comunidad,
            Estado = estado,
            CompaniaSeguros = companiaSeguros,
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            Search = search
        };

        var result = await _service.GetAllAsync(filter);
        return Ok(result);
    }

    /// <summary>Obtiene un siniestro por ID</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SiniestroDto>> GetById(int id)
    {
        var siniestro = await _service.GetByIdAsync(id);
        return siniestro is null ? NotFound() : Ok(siniestro);
    }

    /// <summary>Crea un nuevo siniestro</summary>
    [HttpPost]
    public async Task<ActionResult<SiniestroDto>> Create([FromBody] CreateSiniestroDto dto)
    {
        var created = await _service.CreateAsync(dto, GetUserName());
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Actualiza un siniestro existente</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SiniestroDto>> Update(int id, [FromBody] UpdateSiniestroDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto, GetUserName());
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Elimina un siniestro</summary>
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

    /// <summary>Obtiene lista de compañías de seguros únicas</summary>
    [HttpGet("companias")]
    public async Task<ActionResult<IEnumerable<string>>> GetCompanias()
    {
        var companias = await _service.GetCompaniasAsync();
        return Ok(companias);
    }
}
