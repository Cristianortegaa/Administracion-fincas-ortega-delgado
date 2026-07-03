import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { SiniestroService } from '../../../core/services/siniestro.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  EstadoSiniestro,
  ESTADOS,
  Siniestro,
  SiniestroFilter,
} from '../../../core/models/siniestro.model';
import { SiniestroFormComponent } from '../siniestro-form/siniestro-form.component';
import { SiniestroDetailComponent } from '../siniestro-detail/siniestro-detail.component';
import { StatusBadgeComponent } from '../../../shared/status-badge/status-badge.component';
import { ConfirmModalComponent } from '../../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-siniestros-list',
  standalone: true,
  imports: [
    FormsModule,
    DatePipe,
    SiniestroFormComponent,
    SiniestroDetailComponent,
    StatusBadgeComponent,
    ConfirmModalComponent,
  ],
  templateUrl: './siniestros-list.component.html',
  styleUrl: './siniestros-list.component.scss',
})
export class SiniestrosListComponent implements OnInit {
  private readonly service = inject(SiniestroService);
  readonly toastService    = inject(ToastService);
  private get toast()      { return this.toastService; }

  siniestros   = signal<Siniestro[]>([]);
  comunidades  = signal<string[]>([]);
  companias    = signal<string[]>([]);
  loading       = signal(false);
  deleting      = signal<number | null>(null);
  pendingDelete = signal<Siniestro | null>(null);

  // Modales
  showForm   = signal(false);
  showDetail = signal(false);
  selected   = signal<Siniestro | null>(null);

  // Filtros
  filter: SiniestroFilter = {};
  searchText = '';
  private searchTimer: ReturnType<typeof setTimeout> | null = null;

  readonly estados = ESTADOS;

  // Stats
  get totalAbiertos():    number { return this.siniestros().filter(s => s.estado === 'Abierto').length; }
  get totalEnProceso():   number { return this.siniestros().filter(s => s.estado === 'EnProceso').length; }
  get totalFinalizados(): number { return this.siniestros().filter(s => s.estado === 'Finalizado').length; }

  ngOnInit(): void {
    this.load();
    this.service.getComunidades().subscribe(c => this.comunidades.set(c));
    this.service.getCompanias().subscribe(c  => this.companias.set(c));
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll(this.filter).subscribe({
      next:  (data) => { this.siniestros.set(data); this.loading.set(false); },
      error: ()     => { this.toast.show('Error al cargar los siniestros', 'error'); this.loading.set(false); },
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

  openNew(): void {
    this.selected.set(null);
    this.showForm.set(true);
  }

  openEdit(s: Siniestro): void {
    this.selected.set(s);
    this.showDetail.set(false);
    this.showForm.set(true);
  }

  openDetail(s: Siniestro): void {
    this.selected.set(s);
    this.showDetail.set(true);
  }

  onSaved(s: Siniestro): void {
    this.showForm.set(false);
    this.selected.set(null);
    this.load();
    this.service.getComunidades().subscribe(c => this.comunidades.set(c));
    this.service.getCompanias().subscribe(c  => this.companias.set(c));
  }

  closeForm():   void { this.showForm.set(false);   this.selected.set(null); }
  closeDetail(): void { this.showDetail.set(false); this.selected.set(null); }

  confirmDelete(s: Siniestro, event: Event): void {
    event.stopPropagation();
    this.pendingDelete.set(s);
  }

  cancelDelete(): void { this.pendingDelete.set(null); }

  doDelete(): void {
    const s = this.pendingDelete();
    if (!s) return;
    this.deleting.set(s.id);
    this.service.delete(s.id).subscribe({
      next: () => {
        this.toast.show('Siniestro eliminado');
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

  trackById(_: number, s: Siniestro): number { return s.id; }
}
