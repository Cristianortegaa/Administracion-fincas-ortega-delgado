import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  inject,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SiniestroService } from '../../../core/services/siniestro.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  CreateSiniestroDto,
  DescripcionEntrada,
  EstadoSiniestro,
  ESTADOS,
  Siniestro,
  UpdateSiniestroDto,
} from '../../../core/models/siniestro.model';
import { DescripcionesComponent } from '../../../shared/descripciones/descripciones.component';

@Component({
  selector: 'app-siniestro-form',
  standalone: true,
  imports: [FormsModule, DescripcionesComponent],
  templateUrl: './siniestro-form.component.html',
  styleUrl: './siniestro-form.component.scss',
})
export class SiniestroFormComponent implements OnInit, OnChanges {
  @Input() siniestro: Siniestro | null = null;
  @Output() saved  = new EventEmitter<Siniestro>();
  @Output() closed = new EventEmitter<void>();

  private readonly service = inject(SiniestroService);
  private readonly toast   = inject(ToastService);

  readonly estados = ESTADOS;
  saving = false;

  form: CreateSiniestroDto = this.emptyForm();

  /** Entradas existentes, mostradas como historial en modo edición */
  existingDescripciones: DescripcionEntrada[] = [];

  get isEdit(): boolean { return this.siniestro !== null; }
  get title(): string   { return this.isEdit ? 'Editar Siniestro' : 'Nuevo Siniestro'; }

  ngOnInit(): void  { this.resetForm(); }
  ngOnChanges(): void { this.resetForm(); }

  private resetForm(): void {
    if (this.siniestro) {
      this.existingDescripciones = this.siniestro.detallesSiniestro ?? [];
      this.form = {
        fechaSiniestro:     this.siniestro.fechaSiniestro,
        numeroCDA:          this.siniestro.numeroCDA,
        comunidad:          this.siniestro.comunidad,
        ubicacionDanio:     this.siniestro.ubicacionDanio,
        detallesSiniestro:  '',   // empty — new entry only; existing shown separately
        companiaSeguros:    this.siniestro.companiaSeguros,
        telefonoCompania:   this.siniestro.telefonoCompania,
        referenciaSiniestro: this.siniestro.referenciaSiniestro,
        reparador:          this.siniestro.reparador,
        estado:             this.siniestro.estado,
      };
    } else {
      this.existingDescripciones = [];
      this.form = this.emptyForm();
    }
  }

  private emptyForm(): CreateSiniestroDto {
    return {
      fechaSiniestro:     new Date().toISOString().split('T')[0],
      numeroCDA:          '',
      comunidad:          '',
      ubicacionDanio:     '',
      detallesSiniestro:  '',
      companiaSeguros:    '',
      telefonoCompania:   '',
      referenciaSiniestro: '',
      reparador:          '',
      estado:             'Abierto',
    };
  }

  submit(): void {
    this.saving = true;
    const action$ = this.isEdit
      ? this.service.update(this.siniestro!.id, this.form as UpdateSiniestroDto)
      : this.service.create(this.form);

    action$.subscribe({
      next: (result) => {
        this.toast.show(
          this.isEdit ? 'Siniestro actualizado correctamente' : 'Siniestro creado correctamente'
        );
        this.saved.emit(result);
        this.saving = false;
      },
      error: () => {
        this.toast.show('Error al guardar el siniestro', 'error');
        this.saving = false;
      },
    });
  }

  close(): void { this.closed.emit(); }
}
