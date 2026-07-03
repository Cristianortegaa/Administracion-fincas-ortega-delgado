import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { UserDto, CreateUserDto, UpdateUserDto } from '../../core/models/user.model';
import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-usuarios-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmModalComponent],
  templateUrl: './usuarios-list.component.html',
  styleUrl: './usuarios-list.component.scss',
})
export class UsuariosListComponent implements OnInit {
  private svc = inject(UserService);

  users    = signal<UserDto[]>([]);
  loading  = signal(true);
  error    = signal<string | null>(null);

  // Modal crear
  showModal = signal(false);
  saving    = signal(false);
  formError = signal<string | null>(null);
  form: CreateUserDto = { name: '', email: '', password: '' };

  // Modal editar
  editTarget  = signal<UserDto | null>(null);
  editSaving  = signal(false);
  editError   = signal<string | null>(null);
  editForm: UpdateUserDto = { name: '', newPassword: '' };

  // Confirmación borrado
  deleteTarget = signal<UserDto | null>(null);
  deleting     = signal(false);

  ngOnInit() { this.load(); }

  load() {
    this.loading.set(true);
    this.svc.getAll().subscribe({
      next: (list) => { this.users.set(list); this.loading.set(false); },
      error: () => { this.error.set('Error cargando usuarios.'); this.loading.set(false); },
    });
  }

  openModal() {
    this.form = { name: '', email: '', password: '' };
    this.formError.set(null);
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  submitCreate() {
    this.formError.set(null);
    if (!this.form.name.trim())        { this.formError.set('El nombre es obligatorio.'); return; }
    if (!this.form.email.includes('@')){ this.formError.set('Email no válido.'); return; }
    if (this.form.password.length < 6) { this.formError.set('La contraseña debe tener al menos 6 caracteres.'); return; }

    this.saving.set(true);
    this.svc.create(this.form).subscribe({
      next: (user) => {
        this.users.update(l => [...l, user].sort((a, b) => a.name.localeCompare(b.name)));
        this.saving.set(false);
        this.showModal.set(false);
      },
      error: (err) => {
        this.formError.set(err.error?.message ?? 'Error al crear el usuario.');
        this.saving.set(false);
      },
    });
  }

  openEdit(user: UserDto) {
    this.editForm = { name: user.name, newPassword: '' };
    this.editError.set(null);
    this.editTarget.set(user);
  }

  closeEdit() { this.editTarget.set(null); }

  submitEdit() {
    this.editError.set(null);
    const target = this.editTarget();
    if (!target) return;
    if (!this.editForm.name.trim()) { this.editError.set('El nombre es obligatorio.'); return; }
    if (this.editForm.newPassword && this.editForm.newPassword.length < 6) {
      this.editError.set('La contraseña debe tener al menos 6 caracteres.'); return;
    }

    this.editSaving.set(true);
    this.svc.update(target.id, this.editForm).subscribe({
      next: (updated) => {
        this.users.update(l =>
          l.map(u => u.id === updated.id ? updated : u)
           .sort((a, b) => a.name.localeCompare(b.name))
        );
        this.editTarget.set(null);
        this.editSaving.set(false);
      },
      error: (err) => {
        this.editError.set(err.error?.message ?? 'Error al guardar.');
        this.editSaving.set(false);
      },
    });
  }

  confirmDelete(user: UserDto) { this.deleteTarget.set(user); }
  cancelDelete()               { this.deleteTarget.set(null); }

  doDelete() {
    const target = this.deleteTarget();
    if (!target) return;
    this.deleting.set(true);
    this.svc.delete(target.id).subscribe({
      next: () => {
        this.users.update(l => l.filter(u => u.id !== target.id));
        this.deleteTarget.set(null);
        this.deleting.set(false);
      },
      error: (err) => {
        alert(err.error?.message ?? 'Error eliminando usuario.');
        this.deleting.set(false);
      },
    });
  }

  formatDate(iso: string): string {
    return new Date(iso).toLocaleDateString('es-ES', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
