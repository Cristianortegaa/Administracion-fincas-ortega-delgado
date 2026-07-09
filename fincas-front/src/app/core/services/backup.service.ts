import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BackupSettings {
  backupFolder:  string;
  scheduledTime: string;   // "HH:mm"
  autoEnabled:   boolean;
  maxBackups:    number;
}

export interface BackupFile {
  fileName:    string;
  sizeBytes:   number;
  createdAt:   string;
  displaySize: string;
}

@Injectable({ providedIn: 'root' })
export class BackupService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/backup`;

  /** Create backup and stream it as download */
  downloadNow(): Observable<Blob> {
    return this.http.post(this.baseUrl + '/now', {}, { responseType: 'blob' });
  }

  /** Create backup and save to server folder */
  saveToServer(): Observable<{ message: string; path: string }> {
    return this.http.post<{ message: string; path: string }>(this.baseUrl + '/save', {});
  }

  /** List backup files stored on server */
  getFiles(): Observable<BackupFile[]> {
    return this.http.get<BackupFile[]>(this.baseUrl + '/files');
  }

  /** Download a specific backup file from server */
  downloadFile(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/files/${encodeURIComponent(fileName)}`, {
      responseType: 'blob',
    });
  }

  /** Delete a backup file from server */
  deleteFile(fileName: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/files/${encodeURIComponent(fileName)}`);
  }

  /** Restore database from a .sql backup file */
  restore(file: File): Observable<{ message: string }> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<{ message: string }>(this.baseUrl + '/restore', form);
  }

  getSettings(): Observable<BackupSettings> {
    return this.http.get<BackupSettings>(this.baseUrl + '/settings');
  }

  saveSettings(settings: BackupSettings): Observable<void> {
    return this.http.put<void>(this.baseUrl + '/settings', settings);
  }
}
