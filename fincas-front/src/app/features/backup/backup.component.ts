import { Component, OnInit, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BackupService, BackupFile, BackupSettings } from '../../core/services/backup.service';
import { ConfirmModalComponent } from '../../shared/confirm-modal/confirm-modal.component';

@Component({
  selector: 'app-backup',
  standalone: true,
  imports: [FormsModule, ConfirmModalComponent],
  templateUrl: './backup.component.html',
  styleUrl: './backup.component.scss',
})
export class BackupComponent implements OnInit {
  private readonly svc = inject(BackupService);

  // ── Carpeta PC ────────────────────────────────────────────────────────────────
  /** Handle de la carpeta seleccionada en el PC del admin */
  private pcDirHandle: FileSystemDirectoryHandle | null = null;
  pcFolderName = signal<string | null>(null);
  folderPickerSupported = 'showDirectoryPicker' in window;

  // ── Archivos servidor ─────────────────────────────────────────────────────────
  files        = signal<BackupFile[]>([]);
  filesLoading = signal(false);
  filesError   = signal<string | null>(null);

  // ── Acciones ──────────────────────────────────────────────────────────────────
  downloading  = signal(false);
  savingServer = signal(false);
  actionMsg    = signal<string | null>(null);
  actionError  = signal<string | null>(null);

  // ── Configuración ─────────────────────────────────────────────────────────────
  settingsLoading = signal(false);
  settingsSaving  = signal(false);
  settingsError   = signal<string | null>(null);
  settingsMsg     = signal<string | null>(null);

  settings: BackupSettings = {
    backupFolder:  'backups',
    scheduledTime: '22:00',
    autoEnabled:   true,
    maxBackups:    30,
  };

  // ── Borrado ───────────────────────────────────────────────────────────────────
  pendingDelete = signal<BackupFile | null>(null);
  deleting      = signal(false);

  // ── Lifecycle ─────────────────────────────────────────────────────────────────
  ngOnInit(): void {
    this.loadFiles();
    this.loadSettings();
  }

  // ── Seleccionar carpeta en el PC ──────────────────────────────────────────────
  async pickPcFolder(): Promise<void> {
    try {
      const handle = await (window as any).showDirectoryPicker({ mode: 'readwrite' });
      this.pcDirHandle  = handle;
      this.pcFolderName.set(handle.name);
    } catch (err: any) {
      if (err?.name !== 'AbortError') console.error(err);
    }
  }

  clearPcFolder(): void {
    this.pcDirHandle = null;
    this.pcFolderName.set(null);
  }

  // ── Descargar ahora ───────────────────────────────────────────────────────────
  downloadNow(): void {
    this.downloading.set(true);
    this.clearMessages();
    this.svc.downloadNow().subscribe({
      next: async (blob) => {
        const fileName = `backup_${this.nowSlug()}.sql`;
        await this.saveBlob(blob, fileName);
        this.downloading.set(false);
        this.showMsg('Copia guardada correctamente.');
      },
      error: (err) => {
        this.actionError.set(err.error?.message ?? 'Error al generar la copia de seguridad.');
        this.downloading.set(false);
      },
    });
  }

  // ── Guardar en servidor ───────────────────────────────────────────────────────
  saveToServer(): void {
    this.savingServer.set(true);
    this.clearMessages();
    this.svc.saveToServer().subscribe({
      next: (res) => {
        this.savingServer.set(false);
        this.showMsg(res.message);
        this.loadFiles();
      },
      error: (err) => {
        this.actionError.set(err.error?.message ?? 'Error al guardar en el servidor.');
        this.savingServer.set(false);
      },
    });
  }

  // ── Descargar archivo de la lista ─────────────────────────────────────────────
  downloadFile(file: BackupFile): void {
    this.svc.downloadFile(file.fileName).subscribe({
      next:  async (blob) => this.saveBlob(blob, file.fileName),
      error: ()            => alert('Error al descargar el archivo.'),
    });
  }

  // ── Eliminar archivo ──────────────────────────────────────────────────────────
  confirmDelete(file: BackupFile): void { this.pendingDelete.set(file); }
  cancelDelete(): void                  { this.pendingDelete.set(null); }

  doDelete(): void {
    const file = this.pendingDelete();
    if (!file) return;
    this.deleting.set(true);
    this.svc.deleteFile(file.fileName).subscribe({
      next: () => { this.pendingDelete.set(null); this.deleting.set(false); this.loadFiles(); },
      error: () => { alert('Error al eliminar la copia.'); this.deleting.set(false); },
    });
  }

  // ── Archivos ──────────────────────────────────────────────────────────────────
  loadFiles(): void {
    this.filesLoading.set(true);
    this.filesError.set(null);
    this.svc.getFiles().subscribe({
      next:  (f) => { this.files.set(f); this.filesLoading.set(false); },
      error: ()  => { this.filesError.set('Error al cargar la lista de copias.'); this.filesLoading.set(false); },
    });
  }

  // ── Configuración ─────────────────────────────────────────────────────────────
  loadSettings(): void {
    this.settingsLoading.set(true);
    this.svc.getSettings().subscribe({
      next: (s) => { this.settings = { ...s }; this.settingsLoading.set(false); },
      error: ()  => { this.settingsLoading.set(false); },
    });
  }

  saveSettings(): void {
    this.settingsSaving.set(true);
    this.settingsError.set(null);
    this.settingsMsg.set(null);
    this.svc.saveSettings(this.settings).subscribe({
      next: () => {
        this.settingsSaving.set(false);
        this.settingsMsg.set('Configuración guardada.');
        setTimeout(() => this.settingsMsg.set(null), 3000);
      },
      error: (err) => {
        this.settingsError.set(err.error?.message ?? 'Error al guardar la configuración.');
        this.settingsSaving.set(false);
      },
    });
  }

  // ── Helpers ───────────────────────────────────────────────────────────────────

  /**
   * Guarda un blob:
   *  1. Si hay carpeta seleccionada → directo a esa carpeta, sin diálogo
   *  2. Si no → abre "Guardar como" del navegador
   */
  private async saveBlob(blob: Blob, fileName: string): Promise<void> {
    // Opción 1: carpeta ya seleccionada → guardar directamente sin diálogos
    if (this.pcDirHandle) {
      try {
        const fileHandle = await this.pcDirHandle.getFileHandle(fileName, { create: true });
        const writable   = await fileHandle.createWritable();
        await writable.write(blob);
        await writable.close();
        return;
      } catch (err: any) {
        if (err?.name === 'NotAllowedError') {
          // El permiso caducó (el navegador lo resetea entre sesiones)
          // Intentar repedir permiso
          try {
            await (this.pcDirHandle as any).requestPermission({ mode: 'readwrite' });
            const fileHandle = await this.pcDirHandle.getFileHandle(fileName, { create: true });
            const writable   = await fileHandle.createWritable();
            await writable.write(blob);
            await writable.close();
            return;
          } catch { /* fall through */ }
        }
      }
    }

    // Opción 2: "Guardar como" nativo (Chrome/Edge)
    if ('showSaveFilePicker' in window) {
      try {
        const handle = await (window as any).showSaveFilePicker({
          suggestedName: fileName,
          types: [{ description: 'Archivo SQL', accept: { 'application/sql': ['.sql'] } }],
        });
        const writable = await handle.createWritable();
        await writable.write(blob);
        await writable.close();
        return;
      } catch (err: any) {
        if (err?.name === 'AbortError') return;
      }
    }

    // Opción 3: fallback clásico (Firefox / Safari)
    const url = URL.createObjectURL(blob);
    const a   = Object.assign(document.createElement('a'), { href: url, download: fileName });
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }

  private nowSlug(): string {
    return new Date().toISOString().slice(0, 19).replace(/:/g, '-').replace('T', '_');
  }

  private clearMessages(): void {
    this.actionMsg.set(null);
    this.actionError.set(null);
  }

  private showMsg(msg: string): void {
    this.actionMsg.set(msg);
    setTimeout(() => this.actionMsg.set(null), 4000);
  }

  formatDate(iso: string): string {
    return new Date(iso).toLocaleString('es-ES', {
      day: '2-digit', month: 'short', year: 'numeric',
      hour: '2-digit', minute: '2-digit',
    });
  }
}
