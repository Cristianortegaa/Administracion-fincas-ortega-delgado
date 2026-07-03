import { Component, EventEmitter, Input, Output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Siniestro } from '../../../core/models/siniestro.model';
import { StatusBadgeComponent } from '../../../shared/status-badge/status-badge.component';
import { DescripcionesComponent } from '../../../shared/descripciones/descripciones.component';

@Component({
  selector: 'app-siniestro-detail',
  standalone: true,
  imports: [DatePipe, StatusBadgeComponent, DescripcionesComponent],
  templateUrl: './siniestro-detail.component.html',
  styleUrl: './siniestro-detail.component.scss',
})
export class SiniestroDetailComponent {
  @Input() siniestro!: Siniestro;
  @Output() closed = new EventEmitter<void>();
  @Output() editRequested = new EventEmitter<Siniestro>();

  close(): void { this.closed.emit(); }
  edit(): void  { this.editRequested.emit(this.siniestro); }
}
