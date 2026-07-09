namespace AdministradorFincasOrtegaDelgado.DTOs;

public class ComunidadDto
{
    public int      Id              { get; set; }
    public string   Nombre          { get; set; } = string.Empty;
    public int?     NumeroComunidad { get; set; }
    public string   CIF             { get; set; } = string.Empty;
    public string   Direccion       { get; set; } = string.Empty;
    public string   CompaniaSeguros { get; set; } = string.Empty;
    public string   NumeroPoliza    { get; set; } = string.Empty;
    public string   TelefonoSeguro  { get; set; } = string.Empty;
    public string   Email           { get; set; } = string.Empty;
    public DateTime FechaCreacion     { get; set; }
    public DateTime FechaModificacion { get; set; }
}

public class CreateComunidadDto
{
    public string  Nombre          { get; set; } = string.Empty;
    public int?    NumeroComunidad { get; set; }
    public string  CIF             { get; set; } = string.Empty;
    public string  Direccion       { get; set; } = string.Empty;
    public string  CompaniaSeguros { get; set; } = string.Empty;
    public string  NumeroPoliza    { get; set; } = string.Empty;
    public string  TelefonoSeguro  { get; set; } = string.Empty;
    public string  Email           { get; set; } = string.Empty;
}

public class UpdateComunidadDto : CreateComunidadDto { }
