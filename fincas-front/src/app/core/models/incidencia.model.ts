import { DescripcionEntrada } from './expediente.model';
export type { DescripcionEntrada };

export type EstadoIncidencia = 'Abierta' | 'EnProceso' | 'Resuelta';

export interface Incidencia {
  id: number;
  fechaIncidencia: string;
  comunidad: string;
  ubicacion: string;
  descripcion: DescripcionEntrada[];
  tipo: string;
  reparador: string;
  observaciones: string;
  estado: EstadoIncidencia;
  fechaCreacion: string;
  fechaModificacion: string;
}

export interface CreateIncidenciaDto {
  fechaIncidencia: string;
  comunidad: string;
  ubicacion: string;
  descripcion: string;
  tipo: string;
  reparador: string;
  observaciones: string;
  estado: EstadoIncidencia;
}

export type UpdateIncidenciaDto = CreateIncidenciaDto;

export interface IncidenciaFilter {
  comunidad?: string;
  estado?: EstadoIncidencia | '';
  tipo?: string;
  fechaDesde?: string;
  fechaHasta?: string;
  search?: string;
}

export const ESTADOS_INCIDENCIA: { value: EstadoIncidencia; label: string }[] = [
  { value: 'Abierta',   label: 'Abierta'    },
  { value: 'EnProceso', label: 'En Proceso' },
  { value: 'Resuelta',  label: 'Resuelta'   },
];

export const TIPOS_INCIDENCIA = [
  'Avería', 'Mantenimiento', 'Fontanería', 'Electricidad',
  'Ascensor', 'Fachada', 'Limpieza', 'Vandalismo', 'Otros',
];
