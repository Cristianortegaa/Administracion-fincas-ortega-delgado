import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly auth   = inject(AuthService);
  private readonly router = inject(Router);

  email    = '';
  password = '';
  loading  = signal(false);
  error    = signal('');
  showPass = signal(false);

  submit(): void {
    this.error.set('');
    if (!this.email || !this.password) {
      this.error.set('Por favor introduce email y contraseña.');
      return;
    }
    this.loading.set(true);
    this.auth.login(this.email, this.password).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading.set(false);
        if (err.status === 401) {
          this.error.set('Email o contraseña incorrectos.');
        } else {
          this.error.set('Error al conectar con el servidor. Verifica que el backend esté activo.');
        }
      },
    });
  }
}
