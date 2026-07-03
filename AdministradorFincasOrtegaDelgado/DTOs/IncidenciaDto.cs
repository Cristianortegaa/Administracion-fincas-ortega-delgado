namespace AdministradorFincasOrtegaDelgado.DTOs;

public class IncidenciaDto
{
    public int Id { get; set; }
    public DateOnly FechaIncidencia { get; set; }
    public string Comunidad { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public List<DescripcionEntradaDto> Descripcion { get; set; } = new();
    public string Tipo { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }
}

public class CreateIncidenciaDto
{
    public DateOnly FechaIncidencia { get; set; }
    public string Comunidad { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public string Estado { get; set; } = "Abierta";
}

public class UpdateIncidenciaDto
{
    public DateOnly FechaIncidencia { get; set; }
    public string Comunidad { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Reparador { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class IncidenciaFilterDto
{
    public string? Comunidad { get; set; }
    public string? Estado { get; set; }
    public string? Tipo { get; set; }
    public DateOnly? FechaDesde { get; set; }
    public DateOnly? FechaHasta { get; set; }
    public string? Search { get; set; }
}
