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
import { ExpedienteService } from '../../../core/services/expediente.service';
import { ComunidadService } from '../../../core/services/comunidad.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  CreateExpedienteDto,
  DescripcionEntrada,
  ESTADOS_EXP,
  Expediente,
  TipoExpediente,
  TIPOS_INCIDENCIA,
  UpdateExpedienteDto,
} from '../../../core/models/expediente.model';
import { Comunidad } from '../../../core/models/comunidad.model';
import { DescripcionesComponent } from '../../../shared/descripciones/descripciones.component';

@Component({
  selector: 'app-expediente-form',
  standalone: true,
  imports: [FormsModule, DescripcionesComponent],
  templateUrl: './expediente-form.component.html',
  styleUrl: './expediente-form.component.scss',
})
export class ExpedienteFormComponent implements OnInit, OnChanges {
  @Input() expediente: Expediente | null = null;
  @Input() defaultTipo: TipoExpediente = 'Incidencia';
  @Output() saved  = new EventEmitter<Expediente>();
  @Output() closed = new EventEmitter<void>();

  private readonly service          = inject(ExpedienteService);
  private readonly comunidadService = inject(ComunidadService);
  private readonly toast            = inject(ToastService);

  readonly estados         = ESTADOS_EXP;
  readonly tiposIncidencia = TIPOS_INCIDENCIA;
  saving = false;

  form: CreateExpedienteDto = this.emptyForm();

  /** Entradas existentes, mostradas como historial en modo edición */
  existingDescripciones: DescripcionEntrada[] = [];

  /** Input temporal para añadir un nuevo reparador */
  newReparador = '';

  /** Lista de comunidades para el selector */
  comunidades: Comunidad[] = [];

  get isEdit():      boolean { return this.expediente !== null; }
  get isSiniestro(): boolean { return this.form.tipo === 'Siniestro'; }
  get title():       string  {
    return `${this.isEdit ? 'Editar' : 'Nuevo'} Expediente`;
  }

  ngOnInit(): void {
    this.resetForm();
    this.comunidadService.getAll().subscribe({
      next: (list) => { this.comunidades = list; },
      error: () => {},
    });
  }

  ngOnChanges(): void { this.resetForm(); }

  private resetForm(): void {
    if (this.expediente) {
      this.existingDescripciones = this.expediente.descripcion ?? [];
      this.form = {
        tipo:                this.expediente.tipo,
        fecha:               this.expediente.fecha,
        comunidad:           this.expediente.comunidad,
        numeroComunidad:     this.expediente.numeroComunidad ?? null,
        ubicacion:           this.expediente.ubicacion,
        descripcion:         '',
        tipoIncidencia:      this.expediente.tipoIncidencia,
        reparadores:         [...(this.expediente.reparadores ?? [])],
        observaciones:       this.expediente.observaciones,
        estado:              this.expediente.estado,
        numeroCDA:           this.expediente.numeroCDA ?? null,
        companiaSeguros:     this.expediente.companiaSeguros ?? null,
        numeroPoliza:        this.expediente.numeroPoliza ?? null,
        telefonoCompania:    this.expediente.telefonoCompania ?? null,
        emailSeguro:         this.expediente.emailSeguro ?? null,
        referenciaSiniestro: this.expediente.referenciaSiniestro ?? null,
      };
    } else {
      this.existingDescripciones = [];
      this.form = this.emptyForm();
    }
    this.newReparador = '';
  }

  private emptyForm(): CreateExpedienteDto {
    return {
      tipo:                this.defaultTipo,
      fecha:               new Date().toISOString().split('T')[0],
      comunidad:           '',
      numeroComunidad:     null,
      ubicacion:           '',
      descripcion:         '',
      tipoIncidencia:      '',
      reparadores:         [],
      observaciones:       '',
      estado:              'Abierto',
      numeroCDA:           null,
      companiaSeguros:     null,
      numeroPoliza:        null,
      telefonoCompania:    null,
      emailSeguro:         null,
      referenciaSiniestro: null,
    };
  }

  setTipo(tipo: TipoExpediente): void {
    this.form.tipo = tipo;
    if (tipo === 'Incidencia') {
      this.form.numeroCDA           = null;
      this.form.companiaSeguros     = null;
      this.form.numeroPoliza        = null;
      this.form.telefonoCompania    = null;
      this.form.emailSeguro         = null;
      this.form.referenciaSiniestro = null;
    }
  }

  /** Al seleccionar una comunidad del desplegable, rellena automáticamente los datos del seguro */
  onComunidadChange(nombre: string): void {
    this.form.comunidad = nombre;
    const found = this.comunidades.find(c => c.nombre === nombre);
    if (found && this.form.tipo === 'Siniestro') {
      this.form.companiaSeguros  = found.companiaSeguros  || this.form.companiaSeguros;
      this.form.numeroPoliza     = found.numeroPoliza     || this.form.numeroPoliza;
      this.form.telefonoCompania = found.telefonoSeguro   || this.form.telefonoCompania;
      this.form.emailSeguro      = found.email            || this.form.emailSeguro;
      if (found.numeroComunidad) this.form.numeroComunidad = found.numeroComunidad;
    }
  }

  // ── Gestión de reparadores ─────────────────────────────────
  addReparador(): void {
    const nombre = this.newReparador.trim();
    if (!nombre) return;
    if (!this.form.reparadores.includes(nombre)) {
      this.form.reparadores = [...this.form.reparadores, nombre];
    }
    this.newReparador = '';
  }

  removeReparador(index: number): void {
    this.form.reparadores = this.form.reparadores.filter((_, i) => i !== index);
  }

  onReparadorKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.addReparador();
    }
  }

  submit(): void {
    this.saving = true;
    const action$ = this.isEdit
      ? this.service.update(this.expediente!.id, this.form as UpdateExpedienteDto)
      : this.service.create(this.form);

    action$.subscribe({
      next: (result) => {
        this.toast.show(
          this.isEdit ? 'Expediente actualizado correctamente' : 'Expediente creado correctamente'
        );
        this.saved.emit(result);
        this.saving = false;
      },
      error: () => {
        this.toast.show('Error al guardar el expediente', 'error');
        this.saving = false;
      },
    });
  }

  close(): void { this.closed.emit(); }
}
