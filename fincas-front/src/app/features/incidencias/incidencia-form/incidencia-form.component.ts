import { Component, EventEmitter, Input, OnChanges, OnInit, Output, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { IncidenciaService } from '../../../core/services/incidencia.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  CreateIncidenciaDto,
  DescripcionEntrada,
  ESTADOS_INCIDENCIA,
  TIPOS_INCIDENCIA,
  Incidencia,
  UpdateIncidenciaDto,
} from '../../../core/models/incidencia.model';
import { DescripcionesComponent } from '../../../shared/descripciones/descripciones.component';

@Component({
  selector: 'app-incidencia-form',
  standalone: true,
  imports: [FormsModule, DescripcionesComponent],
  templateUrl: './incidencia-form.component.html',
  styleUrl: './incidencia-form.component.scss',
})
export class IncidenciaFormComponent implements OnInit, OnChanges {
  @Input() incidencia: Incidencia | null = null;
  @Output() saved  = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  private readonly service = inject(IncidenciaService);
  private readonly toast   = inject(ToastService);

  readonly estados = ESTADOS_INCIDENCIA;
  readonly tipos   = TIPOS_INCIDENCIA;
  saving = false;

  form: CreateIncidenciaDto = this.emptyForm();

  /** Entradas existentes, mostradas como historial en modo edición */
  existingDescripciones: DescripcionEntrada[] = [];

  get isEdit(): boolean { return this.incidencia !== null; }
  get title(): string   { return this.isEdit ? 'Editar Incidencia' : 'Nueva Incidencia'; }

  ngOnInit(): void    { this.resetForm(); }
  ngOnChanges(): void { this.resetForm(); }

  private resetForm(): void {
    if (this.incidencia) {
      this.existingDescripciones = this.incidencia.descripcion ?? [];
      this.form = {
        fechaIncidencia: this.incidencia.fechaIncidencia,
        comunidad:       this.incidencia.comunidad,
        ubicacion:       this.incidencia.ubicacion,
        descripcion:     '',   // empty — new entry only; existing shown separately
        tipo:            this.incidencia.tipo,
        reparador:       this.incidencia.reparador,
        observaciones:   this.incidencia.observaciones,
        estado:          this.incidencia.estado,
      };
    } else {
      this.existingDescripciones = [];
      this.form = this.emptyForm();
    }
  }

  private emptyForm(): CreateIncidenciaDto {
    return {
      fechaIncidencia: new Date().toISOString().split('T')[0],
      comunidad:       '',
      ubicacion:       '',
      descripcion:     '',
      tipo:            '',
      reparador:       '',
      observaciones:   '',
      estado:          'Abierta',
    };
  }

  submit(): void {
    this.saving = true;
    const action$ = this.isEdit
      ? this.service.update(this.incidencia!.id, this.form as UpdateIncidenciaDto)
      : this.service.create(this.form);

    action$.subscribe({
      next: () => {
        this.toast.show(this.isEdit ? 'Incidencia actualizada' : 'Incidencia creada');
        this.saved.emit();
        this.saving = false;
      },
      error: () => {
        this.toast.show('Error al guardar la incidencia', 'error');
        this.saving = false;
      },
    });
  }

  close(): void { this.closed.emit(); }
}
