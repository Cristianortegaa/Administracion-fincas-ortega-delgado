export type TipoExpediente   = 'Incidencia' | 'Siniestro';
export type EstadoExpediente = 'Abierto' | 'Cerrado';

export interface DescripcionEntrada {
  texto:   string;
  fecha:   string;   // ISO datetime from backend
  usuario: string;
}

export interface Expediente {
  id:                  number;
  tipo:                TipoExpediente;
  fecha:               string;          // ISO date "YYYY-MM-DD"
  comunidad:           string;
  numeroComunidad?:    number | null;
  ubicacion:           string;
  descripcion:         DescripcionEntrada[];
  tipoIncidencia:      string;
  reparadores:         string[];
  observaciones:       string;
  estado:              EstadoExpediente;
  // Solo siniestro
  numeroCDA?:           string | null;
  companiaSeguros?:     string | null;
  telefonoCompania?:    string | null;
  referenciaSiniestro?: string | null;
  // Auditoría
  creadoPorNombre:      string;
  fechaCreacion:        string;
  modificadoPorNombre:  string;
  fechaModificacion:    string;
}

export interface CreateExpedienteDto {
  tipo:                TipoExpediente;
  fecha:               string;
  comunidad:           string;
  numeroComunidad?:    number | null;
  ubicacion:           string;
  descripcion:         string;
  tipoIncidencia:      string;
  reparadores:         string[];
  observaciones:       string;
  estado:              EstadoExpediente;
  numeroCDA?:           string | null;
  companiaSeguros?:     string | null;
  telefonoCompania?:    string | null;
  referenciaSiniestro?: string | null;
}

export type UpdateExpedienteDto = CreateExpedienteDto;

export interface ExpedienteFilter {
  tipo?:            TipoExpediente | '';
  estado?:          EstadoExpediente | '';
  comunidad?:       string;
  numeroComunidad?: number;
  tipoIncidencia?:  string;
  fechaDesde?:      string;   // ISO "YYYY-MM-DD"
  fechaHasta?:      string;   // ISO "YYYY-MM-DD"
  search?:          string;
  anio?:            number;
  empresa?:         string;
  companiaSeguros?: string;
}

export const ESTADOS_EXP: { value: EstadoExpediente; label: string }[] = [
  { value: 'Abierto', label: 'Abierto' },
  { value: 'Cerrado', label: 'Cerrado' },
];

export const TIPOS_INCIDENCIA: string[] = [
  'Fontanería',
  'Eléctrico',
  'Albañilería',
  'Pintura',
  'Carpintería',
  'Ascensor',
  'Tejado / Cubierta',
  'Jardín',
  'Limpieza',
  'Portería / Acceso',
  'Otro',
];
