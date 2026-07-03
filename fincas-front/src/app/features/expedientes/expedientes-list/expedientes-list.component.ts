import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { ExpedienteService } from '../../../core/services/expediente.service';
import { ToastService } from '../../../core/services/toast.service';
import {
  ESTADOS_EXP,
  Expediente,
  ExpedienteFilter,
  TipoExpediente,
} from '../../../core/models/expediente.model';
import { ExpedienteFormComponent } from '../expediente-form/expediente-form.component';
import { ExpedienteDetailComponent } from '../expediente-detail/expediente-detail.component';
import { StatusBadgeComponent } from '../../../shared/status-badge/status-badge.component';
import { ConfirmModalComponent } from '../../../shared/confirm-modal/confirm-modal.component';

type TabTipo = 'Todos' | TipoExpediente;

@Component({
  selector: 'app-expedientes-list',
  standalone: true,
  imports: [
    FormsModule,
    DatePipe,
    ExpedienteFormComponent,
    ExpedienteDetailComponent,
    StatusBadgeComponent,
    ConfirmModalComponent,
  ],
  templateUrl: './expedientes-list.component.html',
  styleUrl: './expedientes-list.component.scss',
})
export class ExpedientesListComponent implements OnInit {
  private readonly service  = inject(ExpedienteService);
  readonly toastService     = inject(ToastService);

  all             = signal<Expediente[]>([]);
  comunidades     = signal<string[]>([]);
  tiposIncidencia = signal<string[]>([]);
  loading         = signal(false);

  // Años disponibles calculados de los expedientes cargados (sin llamada extra al backend)
  readonly anios = computed<number[]>(() => {
    const years = [...new Set(this.all().map(e => parseInt(e.fecha.substring(0, 4), 10)))];
    return years.sort((a, b) => b - a);
  });

  // Mapa comunidad → numeroComunidad para mostrar el número en filtros y tabla
  readonly comunidadNumMap = computed<Record<string, number | null>>(() => {
    const map: Record<string, number | null> = {};
    for (const e of this.all()) {
      if (e.comunidad && !(e.comunidad in map)) {
        map[e.comunidad] = e.numeroComunidad ?? null;
      }
    }
    return map;
  });

  // Números de comunidad disponibles para el filtro
  readonly numerosDisponibles = computed<number[]>(() => {
    const nums = this.all()
      .map(e => e.numeroComunidad)
      .filter((n): n is number => n != null);
    return [...new Set(nums)].sort((a, b) => a - b);
  });

  // Empresas reparadoras únicas
  readonly empresasDisponibles = computed<string[]>(() => {
    const all = this.all().flatMap(e => e.reparadores).filter(Boolean);
    return [...new Set(all)].sort();
  });

  // Compañías de seguros únicas
  readonly companiasDisponibles = computed<string[]>(() => {
    const all = this.all()
      .map(e => e.companiaSeguros)
      .filter((c): c is string => !!c);
    return [...new Set(all)].sort();
  });

  deleting      = signal<number | null>(null);
  pendingDelete = signal<Expediente | null>(null);

  // ── Selección para imprimir ────────────────────────────────
  selectedIds    = signal<Set<number>>(new Set());
  showPrintModal = signal(false);

  get someSelected(): boolean { return this.selectedIds().size > 0; }

  get allSelected(): boolean {
    const items = this.items();
    return items.length > 0 && items.every(e => this.selectedIds().has(e.id));
  }

  get partialSelected(): boolean { return this.someSelected && !this.allSelected; }

  isSelected(id: number): boolean { return this.selectedIds().has(id); }

  toggleAll(event: Event): void {
    const cb = event.target as HTMLInputElement;
    this.selectedIds.set(cb.checked
      ? new Set(this.items().map(e => e.id))
      : new Set());
  }

  toggleOne(id: number, event: Event): void {
    event.stopPropagation();
    const s = new Set(this.selectedIds());
    s.has(id) ? s.delete(id) : s.add(id);
    this.selectedIds.set(s);
  }

  get selectedExpedientes(): Expediente[] {
    const ids = this.selectedIds();
    return this.items().filter(e => ids.has(e.id));
  }

  openPrintModal():  void { this.showPrintModal.set(true);  }
  closePrintModal(): void { this.showPrintModal.set(false); }

  printListado(): void {
    const exps = this.selectedExpedientes;
    const fecha = new Date().toLocaleDateString('es-ES');
    const rows = exps.map(e => `
      <tr>
        <td>${e.id}</td>
        <td>${e.tipo}</td>
        <td>${this.fmtDate(e.fecha)}</td>
        <td>${e.comunidad}${e.numeroComunidad ? ' (#' + e.numeroComunidad + ')' : ''}</td>
        <td>${e.ubicacion || '—'}</td>
        <td>${e.tipoIncidencia || '—'}</td>
        <td>${e.reparadores?.join(', ') || '—'}</td>
        <td>${e.descripcion?.at(-1)?.texto || '—'}</td>
        <td>${e.estado}</td>
      </tr>`).join('');

    const html = `<!DOCTYPE html><html lang="es"><head><meta charset="UTF-8">
<title>Listado de Expedientes</title>
<style>
  body{font-family:Arial,sans-serif;font-size:11px;color:#222;margin:20px}
  h1{font-size:15px;margin-bottom:2px} .sub{color:#777;font-size:10px;margin-bottom:14px}
  table{width:100%;border-collapse:collapse}
  th{background:#f5f0e0;text-align:left;padding:5px 7px;border-bottom:2px solid #B8920A;font-size:10px}
  td{padding:4px 7px;border-bottom:1px solid #eee;vertical-align:top}
  tr:nth-child(even) td{background:#fafafa}
  .footer{margin-top:14px;font-size:9px;color:#aaa}
</style></head><body>
<h1>Listado de Expedientes</h1>
<p class="sub">Generado el ${fecha} · ${exps.length} expediente${exps.length !== 1 ? 's' : ''}</p>
<table><thead><tr>
  <th>#</th><th>Tipo</th><th>Fecha</th><th>Comunidad</th><th>Ubicación</th>
  <th>Tipo incid.</th><th>Empresa(s)</th><th>Descripción</th><th>Estado</th>
</tr></thead><tbody>${rows}</tbody></table>
<p class="footer">AdministradorFincasOrtegaDelgado · ${fecha}</p>
<script>window.onload=()=>window.print();</script>
</body></html>`;

    this.openPrintWindow(html);
    this.closePrintModal();
  }

  printDetalle(): void {
    const exps = this.selectedExpedientes;
    const fecha = new Date().toLocaleDateString('es-ES');

    const cards = exps.map((e, idx) => {
      const descs = (e.descripcion || [])
        .filter(d => d.texto?.trim())
        .sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime())
        .map(d => {
          const fd = (d.fecha && d.fecha !== '0001-01-01T00:00:00')
            ? new Date(d.fecha).toLocaleString('es-ES', {day:'2-digit',month:'2-digit',year:'numeric',hour:'2-digit',minute:'2-digit'})
            : 'Fecha no registrada';
          return `<div class="de"><div class="dm">${d.usuario || 'Sistema'} · ${fd}</div><div>${d.texto}</div></div>`;
        }).join('');

      const seguro = e.tipo === 'Siniestro' ? `
        <div class="f"><span class="l">N.º CDAD</span><span>${e.numeroCDA || '—'}</span></div>
        <div class="f"><span class="l">Compañía</span><span>${e.companiaSeguros || '—'}</span></div>
        <div class="f"><span class="l">Referencia</span><span>${e.referenciaSiniestro || '—'}</span></div>` : '';

      return `${idx > 0 ? '<div class="pb"></div>' : ''}
<div class="card">
  <div class="ch">
    <span class="tb">${e.tipo === 'Siniestro' ? '🛡' : '🔧'} ${e.tipo}</span>
    <strong>Expediente #${e.id}</strong>
    <span class="est">${e.estado}</span>
  </div>
  <div class="grid">
    <div class="f"><span class="l">Fecha</span><span>${this.fmtDate(e.fecha)}</span></div>
    <div class="f"><span class="l">Comunidad</span><span>${e.comunidad}${e.numeroComunidad ? ' (#'+e.numeroComunidad+')' : ''}</span></div>
    <div class="f"><span class="l">Ubicación</span><span>${e.ubicacion || '—'}</span></div>
    <div class="f"><span class="l">Tipo incidencia</span><span>${e.tipoIncidencia || '—'}</span></div>
    <div class="f"><span class="l">Empresa(s)</span><span>${e.reparadores?.join(', ') || '—'}</span></div>
    <div class="f"><span class="l">Abierto por</span><span>${e.creadoPorNombre || '—'}</span></div>
    ${seguro}
  </div>
  ${descs ? `<div class="ds"><div class="st">Historial de descripciones</div>${descs}</div>` : ''}
  ${e.observaciones ? `<div class="os"><div class="st">Observaciones</div><p>${e.observaciones}</p></div>` : ''}
</div>`;
    }).join('');

    const html = `<!DOCTYPE html><html lang="es"><head><meta charset="UTF-8">
<title>Detalle de Expedientes</title>
<style>
  body{font-family:Arial,sans-serif;font-size:11px;color:#222;margin:20px}
  h1{font-size:15px;margin-bottom:2px} .sub{color:#777;font-size:10px;margin-bottom:14px}
  .card{border:1px solid #ddd;border-radius:6px;padding:14px;margin-bottom:18px}
  .ch{display:flex;gap:10px;align-items:center;margin-bottom:10px;border-bottom:1px solid #eee;padding-bottom:8px;font-size:12px}
  .tb{background:#f5f0e0;color:#8C6E07;padding:2px 7px;border-radius:4px;font-size:10px}
  .est{margin-left:auto;background:#e8f5e9;color:#2e7d32;padding:2px 7px;border-radius:4px;font-size:10px}
  .grid{display:grid;grid-template-columns:repeat(3,1fr);gap:7px;margin-bottom:10px}
  .f{display:flex;flex-direction:column} .l{font-size:9px;color:#999;text-transform:uppercase;margin-bottom:1px}
  .st{font-size:10px;font-weight:bold;color:#B8920A;margin-bottom:6px}
  .ds{margin-top:8px;border-top:1px solid #eee;padding-top:8px}
  .de{margin-bottom:7px;padding-left:8px;border-left:2px solid #B8920A}
  .dm{font-size:9px;color:#888;margin-bottom:2px}
  .os{margin-top:8px;border-top:1px solid #eee;padding-top:8px}
  .pb{page-break-after:always}
  .footer{margin-top:14px;font-size:9px;color:#aaa}
  @media print{.pb{page-break-after:always}}
</style></head><body>
<h1>Detalle de Expedientes</h1>
<p class="sub">Generado el ${fecha} · ${exps.length} expediente${exps.length !== 1 ? 's' : ''}</p>
${cards}
<p class="footer">AdministradorFincasOrtegaDelgado · ${fecha}</p>
<script>window.onload=()=>window.print();</script>
</body></html>`;

    this.openPrintWindow(html);
    this.closePrintModal();
  }

  private openPrintWindow(html: string): void {
    const win = window.open('', '_blank');
    if (win) { win.document.write(html); win.document.close(); }
  }

  private fmtDate(d: string): string {
    if (!d) return '—';
    return new Date(d).toLocaleDateString('es-ES');
  }

  // Tab activo: Todos / Incidencias / Siniestros
  activeTab = signal<TabTipo>('Todos');

  // Ordenación: false = más recientes primero (por defecto), true = más antiguos primero
  sortAsc = signal(false);
  toggleSort() { this.sortAsc.update(v => !v); }

  // Filtros
  filter: ExpedienteFilter = {};
  searchText = '';
  private searchTimer: ReturnType<typeof setTimeout> | null = null;

  // Modales
  showForm   = signal(false);
  showDetail = signal(false);
  selected   = signal<Expediente | null>(null);

  readonly estados = ESTADOS_EXP;

  // Lista filtrada por tab y ordenada por fecha de creación
  readonly items = computed<Expediente[]>(() => {
    const tab = this.activeTab();
    const asc = this.sortAsc();
    let data = this.all();
    if (tab !== 'Todos') data = data.filter(e => e.tipo === tab);
    return [...data].sort((a, b) => {
      const diff = new Date(a.fechaCreacion).getTime() - new Date(b.fechaCreacion).getTime();
      return asc ? diff : -diff;
    });
  });

  // Stats
  get totalTodos():        number { return this.all().length; }
  get totalIncidencias():  number { return this.all().filter(e => e.tipo === 'Incidencia').length; }
  get totalSiniestros():   number { return this.all().filter(e => e.tipo === 'Siniestro').length; }
  get totalAbiertos(): number { return this.items().filter(e => e.estado === 'Abierto').length; }
  get totalCerrados(): number { return this.items().filter(e => e.estado === 'Cerrado').length; }

  ngOnInit(): void {
    this.load();
    this.service.getComunidades().subscribe(c => this.comunidades.set(c));
    this.service.getTiposIncidencia().subscribe(t => this.tiposIncidencia.set(t));
  }

  load(): void {
    this.loading.set(true);
    this.service.getAll(this.filter).subscribe({
      next:  (data) => { this.all.set(data); this.loading.set(false); },
      error: ()     => { this.toastService.show('Error al cargar expedientes', 'error'); this.loading.set(false); },
    });
  }

  onSearchChange(): void {
    if (this.searchTimer) clearTimeout(this.searchTimer);
    this.searchTimer = setTimeout(() => {
      this.filter.search = this.searchText || undefined;
      this.load();
    }, 350);
  }

  onFilterChange(): void { this.selectedIds.set(new Set()); this.load(); }

  clearFilters(): void {
    this.filter = {};
    this.searchText = '';
    this.selectedIds.set(new Set());
    this.load();
  }

  get hasActiveFilters(): boolean {
    return !!(
      this.filter.estado          ||
      this.filter.comunidad       ||
      this.filter.numeroComunidad ||
      this.filter.tipoIncidencia  ||
      this.filter.fechaDesde      ||
      this.filter.fechaHasta      ||
      this.filter.anio            ||
      this.filter.empresa         ||
      this.filter.companiaSeguros ||
      this.searchText
    );
  }

  setTab(tab: TabTipo): void { this.activeTab.set(tab); this.selectedIds.set(new Set()); }

  get defaultTipoForNew(): TipoExpediente {
    const tab = this.activeTab();
    return tab === 'Todos' ? 'Incidencia' : tab;
  }

  openNew(): void {
    this.selected.set(null);
    this.showForm.set(true);
  }

  openEdit(e: Expediente): void {
    this.selected.set(e);
    this.showDetail.set(false);
    this.showForm.set(true);
  }

  openDetail(e: Expediente): void {
    this.selected.set(e);
    this.showDetail.set(true);
  }

  onSaved(e: Expediente): void {
    this.showForm.set(false);
    this.selected.set(null);
    this.load();
    this.service.getComunidades().subscribe(c => this.comunidades.set(c));
  }

  onTipoChanged(e: Expediente): void {
    // Actualiza en local sin cerrar el modal
    this.selected.set(e);
    // Recarga la lista en background
    this.load();
  }

  closeForm():   void { this.showForm.set(false);   this.selected.set(null); }
  closeDetail(): void { this.showDetail.set(false); this.selected.set(null); }

  confirmDelete(e: Expediente, event: Event): void {
    event.stopPropagation();
    this.pendingDelete.set(e);
  }

  cancelDelete(): void { this.pendingDelete.set(null); }

  doDelete(): void {
    const e = this.pendingDelete();
    if (!e) return;
    this.deleting.set(e.id);
    this.service.delete(e.id).subscribe({
      next: () => {
        this.toastService.show('Expediente eliminado');
        this.deleting.set(null);
        this.pendingDelete.set(null);
        this.load();
      },
      error: () => {
        this.toastService.show('Error al eliminar', 'error');
        this.deleting.set(null);
        this.pendingDelete.set(null);
      },
    });
  }

  trackById(_: number, e: Expediente): number { return e.id; }
}
