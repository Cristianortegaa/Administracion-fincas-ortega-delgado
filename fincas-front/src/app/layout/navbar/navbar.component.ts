import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [DatePipe, FormsModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent {
  readonly auth  = inject(AuthService);
  readonly today = new Date();
  private  elRef = inject(ElementRef);

  showDropdown    = signal(false);
  showChangeModal = signal(false);

  cpCurrent = '';
  cpNew     = '';
  cpConfirm = '';
  cpLoading = signal(false);
  cpError   = signal<string | null>(null);
  cpSuccess = signal(false);

  // Cierra el dropdown si se hace clic fuera del componente
  @HostListener('document:click', ['$event'])
  onDocClick(event: Event): void {
    if (this.showDropdown() && !this.elRef.nativeElement.contains(event.target)) {
      this.showDropdown.set(false);
    }
  }

  toggleDropdown(): void {
    this.showDropdown.update(v => !v);
  }

  openChangePassword(): void {
    this.showDropdown.set(false);
    this.cpCurrent = '';
    this.cpNew     = '';
    this.cpConfirm = '';
    this.cpError.set(null);
    this.cpSuccess.set(false);
    this.showChangeModal.set(true);
  }

  closeChangeModal(): void {
    if (this.cpLoading()) return;
    this.showChangeModal.set(false);
  }

  submitChangePassword(): void {
    if (!this.cpCurrent || !this.cpNew || !this.cpConfirm) {
      this.cpError.set('Todos los campos son obligatorios.');
      return;
    }
    if (this.cpNew.length < 6) {
      this.cpError.set('La nueva contraseña debe tener al menos 6 caracteres.');
      return;
    }
    if (this.cpNew !== this.cpConfirm) {
      this.cpError.set('Las contraseñas nuevas no coinciden.');
      return;
    }
    this.cpLoading.set(true);
    this.cpError.set(null);
    this.auth.changePassword(this.cpCurrent, this.cpNew).subscribe({
      next: () => {
        this.cpLoading.set(false);
        this.cpSuccess.set(true);
        setTimeout(() => this.closeChangeModal(), 1800);
      },
      error: (err) => {
        this.cpLoading.set(false);
        this.cpError.set(err?.error?.message ?? 'Error al cambiar la contraseña.');
      },
    });
  }
}
