export interface Comunidad {
  id:               number;
  nombre:           string;
  numeroComunidad?: number | null;
  cif:              string;
  direccion:        string;
  companiaSeguros:  string;
  numeroPoliza:     string;
  telefonoSeguro:   string;
  fechaCreacion:    string;
  fechaModificacion: string;
}

export interface CreateComunidadDto {
  nombre:           string;
  numeroComunidad?: number | null;
  cif:              string;
  direccion:        string;
  companiaSeguros:  string;
  numeroPoliza:     string;
  telefonoSeguro:   string;
}

export type UpdateComunidadDto = CreateComunidadDto;
