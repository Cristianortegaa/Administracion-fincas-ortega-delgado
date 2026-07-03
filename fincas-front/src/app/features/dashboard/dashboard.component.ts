import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ExpedienteService } from '../../core/services/expediente.service';
import { Expediente } from '../../core/models/expediente.model';
import { StatusBadgeComponent } from '../../shared/status-badge/status-badge.component';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink, DatePipe, StatusBadgeComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private readonly expService = inject(ExpedienteService);
  readonly auth               = inject(AuthService);

  expedientes = signal<Expediente[]>([]);
  loading     = signal(true);
  readonly today = new Date();

  // Stats globales
  get expTotal()       { return this.expedientes().length; }
  get expIncidencias() { return this.expedientes().filter(e => e.tipo === 'Incidencia').length; }
  get expSiniestros()  { return this.expedientes().filter(e => e.tipo === 'Siniestro').length; }
  get expAbiertos() { return this.expedientes().filter(e => e.estado === 'Abierto').length; }
  get expCerrados() { return this.expedientes().filter(e => e.estado === 'Cerrado').length; }

  // Recientes
  get recientes() { return [...this.expedientes()].slice(0, 6); }

  get greeting(): string {
    const name = this.auth.currentUser()?.name;
    return name ? name.split(' ')[0] : 'Administrador';
  }

  ngOnInit(): void {
    this.expService.getAll().subscribe({
      next:  d => { this.expedientes.set(d); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  pct(value: number, total: number): number {
    return total ? Math.round(value / total * 100) : 0;
  }
}
