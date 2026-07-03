import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Expediente, TipoExpediente } from '../../../core/models/expediente.model';
import { ExpedienteService } from '../../../core/services/expediente.service';
import { ToastService } from '../../../core/services/toast.service';
import { StatusBadgeComponent } from '../../../shared/status-badge/status-badge.component';
import { DescripcionesComponent } from '../../../shared/descripciones/descripciones.component';

@Component({
  selector: 'app-expediente-detail',
  standalone: true,
  imports: [DatePipe, StatusBadgeComponent, DescripcionesComponent],
  templateUrl: './expediente-detail.component.html',
  styleUrl: './expediente-detail.component.scss',
})
export class ExpedienteDetailComponent {
  @Input() expediente!: Expediente;
  @Output() closed         = new EventEmitter<void>();
  @Output() editRequested  = new EventEmitter<Expediente>();
  @Output() tipoChanged    = new EventEmitter<Expediente>();

  private readonly service = inject(ExpedienteService);
  private readonly toast   = inject(ToastService);

  converting = signal(false);

  get isSiniestro(): boolean { return this.expediente.tipo === 'Siniestro'; }

  get tipoOpuesto(): TipoExpediente {
    return this.isSiniestro ? 'Incidencia' : 'Siniestro';
  }

  get tipoOpuestoLabel(): string {
    return this.isSiniestro ? 'Incidencia' : 'Siniestro';
  }

  convertirTipo(): void {
    const nuevoTipo = this.tipoOpuesto;
    const msg = this.isSiniestro
      ? `¿Convertir este siniestro a Incidencia? Se perderán los datos del seguro.`
      : `¿Convertir esta incidencia a Siniestro?`;

    if (!confirm(msg)) return;

    this.converting.set(true);
    this.service.cambiarTipo(this.expediente.id, nuevoTipo).subscribe({
      next: (updated) => {
        this.toast.show(`Expediente convertido a ${nuevoTipo} correctamente`);
        this.converting.set(false);
        this.tipoChanged.emit(updated);
      },
      error: () => {
        this.toast.show('Error al cambiar el tipo', 'error');
        this.converting.set(false);
      },
    });
  }

  close(): void { this.closed.emit(); }
  edit():  void { this.editRequested.emit(this.expediente); }
}
