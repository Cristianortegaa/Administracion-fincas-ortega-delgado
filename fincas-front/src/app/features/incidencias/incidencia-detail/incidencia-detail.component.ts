import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Incidencia } from '../../../core/models/incidencia.model';
import { StatusBadgeComponent } from '../../../shared/status-badge/status-badge.component';
import { DescripcionesComponent } from '../../../shared/descripciones/descripciones.component';

@Component({
  selector: 'app-incidencia-detail',
  standalone: true,
  imports: [DatePipe, StatusBadgeComponent, DescripcionesComponent],
  templateUrl: './incidencia-detail.component.html',
  styleUrl: './incidencia-detail.component.scss',
})
export class IncidenciaDetailComponent {
  @Input() incidencia!: Incidencia;
  @Output() closed = new EventEmitter<void>();
  @Output() editRequested = new EventEmitter<Incidencia>();

  close(): void { this.closed.emit(); }
  edit(): void  { this.editRequested.emit(this.incidencia); }
}
