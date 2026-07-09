import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComunidadService } from '../../../core/services/comunidad.service';
import { Comunidad, CreateComunidadDto } from '../../../core/models/comunidad.model';
import { ConfirmModalComponent } from '../../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-comunidades-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmModalComponent],
  templateUrl: './comunidades-list.component.html',
  styleUrl: './comunidades-list.component.scss',
})
export class ComunidadesListComponent implements OnInit {
  private svc = inject(ComunidadService);

  comunidades = signal<Comunidad[]>([]);
  loading     = signal(true);
  error       = signal<string | null>(null);

  // Modal crear/editar
  showModal   = signal(false);
  editTarget  = signal<Comunidad | null>(null);
  saving      = signal(false);
  formError   = signal<string | null>(null);

  form: CreateComunidadDto = this.emptyForm();

  // Confirmación borrado
  deleteTarget = signal<Comunidad | null>(null);
  deleting     = signal(false);

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getAll().subscribe({
      next:  (list) => { this.comunidades.set(list); this.loading.set(false); },
      error: ()     => { this.error.set('Error cargando comunidades.'); this.loading.set(false); },
    });
  }

  openCreate() {
    this.editTarget.set(null);
    this.form = this.emptyForm();
    this.formError.set(null);
    this.showModal.set(true);
  }

  openEdit(c: Comunidad) {
    this.editTarget.set(c);
    this.form = {
      nombre:          c.nombre,
      numeroComunidad: c.numeroComunidad ?? null,
      cif:             c.cif,
      direccion:       c.direccion,
      companiaSeguros: c.companiaSeguros,
      numeroPoliza:    c.numeroPoliza,
      telefonoSeguro:  c.telefonoSeguro,
      email:           c.email,
    };
    this.formError.set(null);
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  submit() {
    if (!this.form.nombre.trim()) { this.formError.set('El nombre es obligatorio.'); return; }
    this.formError.set(null);
    this.saving.set(true);

    const target = this.editTarget();
    const action$ = target
      ? this.svc.update(target.id, this.form)
      : this.svc.create(this.form);

    action$.subscribe({
      next: (result) => {
        if (target) {
          this.comunidades.update(l => l.map(c => c.id === result.id ? result : c));
        } else {
          this.comunidades.update(l => [...l, result]);
        }
        this.saving.set(false);
        this.showModal.set(false);
      },
      error: (err) => {
        this.formError.set(err.error?.message ?? 'Error al guardar.');
        this.saving.set(false);
      },
    });
  }

  confirmDelete(c: Comunidad) { this.deleteTarget.set(c); }
  cancelDelete()               { this.deleteTarget.set(null); }

  doDelete() {
    const target = this.deleteTarget();
    if (!target) return;
    this.deleting.set(true);
    this.svc.delete(target.id).subscribe({
      next: () => {
        this.comunidades.update(l => l.filter(c => c.id !== target.id));
        this.deleteTarget.set(null);
        this.deleting.set(false);
      },
      error: () => {
        alert('Error eliminando comunidad.');
        this.deleting.set(false);
      },
    });
  }

  get modalTitle(): string {
    return this.editTarget() ? 'Editar Comunidad' : 'Nueva Comunidad';
  }

  private emptyForm(): CreateComunidadDto {
    return { nombre: '', numeroComunidad: null, cif: '', direccion: '', companiaSeguros: '', numeroPoliza: '', telefonoSeguro: '', email: '' };
  }
}
