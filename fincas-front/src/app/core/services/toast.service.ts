import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error';
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  toasts = signal<Toast[]>([]);
  private nextId = 0;

  show(message: string, type: 'success' | 'error' = 'success', duration = 3500): void {
    const toast: Toast = { id: this.nextId++, message, type };
    this.toasts.update(t => [...t, toast]);
    setTimeout(() => this.remove(toast.id), duration);
  }

  remove(id: number): void {
    this.toasts.update(t => t.filter(x => x.id !== id));
  }
}
