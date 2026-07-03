import { Component, Input, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { DescripcionEntrada } from '../../core/models/expediente.model';

@Component({
  selector: 'app-descripciones',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './descripciones.component.html',
  styleUrl: './descripciones.component.scss',
})
export class DescripcionesComponent {
  @Input() entradas: DescripcionEntrada[] = [];
  @Input() emptyText = 'Sin descripción registrada.';

  expanded = signal(true);

  get hasEntradas(): boolean {
    return this.entradas?.some(d => d.texto?.trim()) ?? false;
  }

  get sortedEntradas(): DescripcionEntrada[] {
    return [...this.entradas]
      .filter(d => d.texto?.trim())
      .sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime());
  }

  toggle(): void {
    this.expanded.set(!this.expanded());
  }
}
