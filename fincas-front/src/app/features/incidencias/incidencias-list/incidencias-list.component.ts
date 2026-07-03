import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { IncidenciaService } from '../../../core/services/incidencia.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  ESTADOS_INCIDENCIA,
  Incidencia,
  IncidenciaFilter,
} from '../../../core/models/incidencia.model';
import { IncidenciaFormComponent } from '../incidencia-form/incidencia-form.component';
import { IncidenciaDetailComponent } from '../incidencia-detail/incidencia-detail.component';
import { StatusBadgeComponent } from '../../../shared/status-badge/status-badge.component';
import { ConfirmModalComponent } from '../../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-incidencias-list',
  standalone: true,
  imports: [FormsModule, DatePipe, IncidenciaFormComponent, IncidenciaDetailComponent, StatusBadgeComponent, ConfirmModalComponent],
  templateUrl: './incidencias-list.component.html',
  styleUrl: './incidencias-list.component.scss',
})
export class IncidenciasListComponent implements OnInit {
  private readonly service = inject(IncidenciaService);
  readonly toastService    = inject(ToastService);
  private get toast()      { return this.toastService; }

  incidencias  = signal<Incidencia[]>([]);
  comunidades  = signal<string[]>([]);
  tipos        = signal<string[]>([]);
  loading       = signal(false);
  deleting      = signal<number | null>(null);
  pendingDelete = signal<Incidencia | null>(null);

  showForm   = signal(false);
  showDetail = signal(false);
  selected   = signal<Incidencia | null>(null);

  filter: IncidenciaFilter = {};
  searchText = '';
  private searchTimer: ReturnType<typeof setTimeout> | null = null;

  readonly estados = ESTADOS_INCIDENCIA;

  get totalAbiertas():   number { return this.incidencias().filter(i => i.estado === 'Abierta').length; }
  get totalEnProceso():  number { return this.incidencias().filter(i => i.estado === 'EnProceso').length; }
  get totalResueltas():  number { return this.incidencias().filter(i => i.estado === 'Resuelta').length; }

  ngOnInit(): void {
    this.load();
    this.service.getComunidades().subscribe(c => this.comunidades.set(c));
    this.service.getTipos().subscribe(t => this.tipos.set(t));
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll(this.filter).subscribe({
      next:  (data) => { this.incidencias.set(data); this.loading.set(false); },
      error: ()     => { this.toast.show('Error al cargar las incidencias', 'error'); this.loading.set(false); },
    });
  }

  onSearchChange(): void {
    if (this.searchTimer) clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => {
      this.filter.search = this.searchText || undefined;
      this.load();
    }, 350);
  }

  onFilterChange(): void { this.load(); }

  clearFilters(): void {
    this.filter = {};
    this.searchText = '';
    this.load();
  }

  openNew(): void    { this.selected.set(null); this.showForm.set(true); }
  openEdit(i: Incidencia): void { this.selected.set(i); this.showDetail.set(false); this.showForm.set(true); }
  openDetail(i: Incidencia): void { this.selected.set(i); this.showDetail.set(true); }

  onSaved(): void {
    this.showForm.set(false);
    this.selected.set(null);
    this.load();
    this.service.getComunidades().subscribe(c => this.comunidades.set(c));
    this.service.getTipos().subscribe(t => this.tipos.set(t));
  }

  closeForm():   void { this.showForm.set(false);   this.selected.set(null); }
  closeDetail(): void { this.showDetail.set(false); this.selected.set(null); }

  confirmDelete(i: Incidencia, event: Event): void {
    event.stopPropagation();
    this.pendingDelete.set(i);
  }

  cancelDelete(): void { this.pendingDelete.set(null); }

  doDelete(): void {
    const i = this.pendingDelete();
    if (!i) return;
    this.deleting.set(i.id);
    this.service.delete(i.id).subscribe({
      next: () => {
        this.toast.show('Incidencia eliminada');
        this.deleting.set(null);
        this.pendingDelete.set(null);
        this.load();
      },
      error: () => {
        this.toast.show('Error al eliminar', 'error');
        this.deleting.set(null);
        this.pendingDelete.set(null);
      },
    });
  }

  trackById(_: number, i: Incidencia): number { return i.id; }
}
