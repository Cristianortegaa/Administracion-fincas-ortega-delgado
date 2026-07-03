import { DescripcionEntrada } from './expediente.model';
export type { DescripcionEntrada };

export type EstadoSiniestro = 'Abierto' | 'EnProceso' | 'Finalizado';

export interface Siniestro {
  id: number;
  fechaSiniestro: string;   // ISO date "YYYY-MM-DD"
  numeroCDA: string;
  comunidad: string;
  ubicacionDanio: string;
  detallesSiniestro: DescripcionEntrada[];
  companiaSeguros: string;
  telefonoCompania: string;
  referenciaSiniestro: string;
  reparador: string;
  estado: EstadoSiniestro;
  fechaCreacion: string;
  fechaModificacion: string;
}

export interface CreateSiniestroDto {
  fechaSiniestro: string;
  numeroCDA: string;
  comunidad: string;
  ubicacionDanio: string;
  detallesSiniestro: string;
  companiaSeguros: string;
  telefonoCompania: string;
  referenciaSiniestro: string;
  reparador: string;
  estado: EstadoSiniestro;
}

export type UpdateSiniestroDto = CreateSiniestroDto;

export interface SiniestroFilter {
  comunidad?: string;
  estado?: EstadoSiniestro | '';
  companiaSeguros?: string;
  fechaDesde?: string;
  fechaHasta?: string;
  search?: string;
}

export const ESTADOS: { value: EstadoSiniestro; label: string }[] = [
  { value: 'Abierto',    label: 'Abierto'    },
  { value: 'EnProceso',  label: 'En Proceso' },
  { value: 'Finalizado', label: 'Finalizado' },
];
