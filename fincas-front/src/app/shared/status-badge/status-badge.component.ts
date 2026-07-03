import { Component, Input } from '@angular/core';
import { EstadoSiniestro } from '../../core/models/siniestro.model';
import { EstadoIncidencia } from '../../core/models/incidencia.model';

type EstadoUnion = EstadoSiniestro | EstadoIncidencia | 'Cerrado';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  template: `<span class="badge" [class]="badgeClass">{{ label }}</span>`,
})
export class StatusBadgeComponent {
  @Input() estado!: EstadoUnion;

  get badgeClass(): string {
    switch (this.estado) {
      case 'Abierto':
      case 'Abierta':    return 'badge badge-abierto';
      case 'EnProceso':  return 'badge badge-proceso';
      case 'Cerrado':
      case 'Finalizado':
      case 'Resuelta':   return 'badge badge-cerrado';
      default:           return 'badge badge-abierto';
    }
  }

  get label(): string {
    switch (this.estado) {
      case 'Abierto':    return 'Abierto';
      case 'Abierta':    return 'Abierta';
      case 'EnProceso':  return 'En Proceso';
      case 'Cerrado':    return 'Cerrado';
      case 'Finalizado': return 'Finalizado';
      case 'Resuelta':   return 'Resuelta';
      default:           return this.estado;
    }
  }
}
