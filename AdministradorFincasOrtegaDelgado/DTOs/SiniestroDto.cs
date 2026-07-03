namespace AdministradorFincasOrtegaDelgado.DTOs;

public class SiniestroDto
{
    public int Id { get; set; }
    public DateOnly FechaSiniestro { get; set; }
    public string NumeroCDA { get; set; } = string.Empty;
    public string Comunidad { get; set; } = string.Empty;
    public string UbicacionDanio { get; set; } = string.Empty;
    public List<DescripcionEntradaDto> DetallesSiniestro { get; set; } = new();
    public string CompaniaSeguros { get; set; } = string.Empty;
    public string TelefonoCompania { get; set; } = string.Empty;
    public string ReferenciaSiniestro { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }
}

public class CreateSiniestroDto
{
    public DateOnly FechaSiniestro { get; set; }
    public string NumeroCDA { get; set; } = string.Empty;
    public string Comunidad { get; set; } = string.Empty;
    public string UbicacionDanio { get; set; } = string.Empty;
    public string DetallesSiniestro { get; set; } = string.Empty;
    public string CompaniaSeguros { get; set; } = string.Empty;
    public string TelefonoCompania { get; set; } = string.Empty;
    public string ReferenciaSiniestro { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public string Estado { get; set; } = "Abierto";
}

public class UpdateSiniestroDto
{
    public DateOnly FechaSiniestro { get; set; }
    public string NumeroCDA { get; set; } = string.Empty;
    public string Comunidad { get; set; } = string.Empty;
    public string UbicacionDanio { get; set; } = string.Empty;
    public string DetallesSiniestro { get; set; } = string.Empty;
    public string CompaniaSeguros { get; set; } = string.Empty;
    public string TelefonoCompania { get; set; } = string.Empty;
    public string ReferenciaSiniestro { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class SiniestroFilterDto
{
    public string? Comunidad { get; set; }
    public string? Estado { get; set; }
    public string? CompaniaSeguros { get; set; }
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public string? Search { get; set; }
}
