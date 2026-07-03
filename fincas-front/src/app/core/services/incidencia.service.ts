import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Incidencia,
  CreateIncidenciaDto,
  UpdateIncidenciaDto,
  IncidenciaFilter,
} from '../models/incidencia.model';

@Injectable({ providedIn: 'root' })
export class IncidenciaService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/incidencia`;

  getAll(filter?: IncidenciaFilter): Observable<Incidencia[]> {
    let params = new HttpParams();
    if (filter) {
      if (filter.comunidad)  params = params.set('comunidad',  filter.comunidad);
      if (filter.estado)     params = params.set('estado',     filter.estado);
      if (filter.tipo)       params = params.set('tipo',       filter.tipo);
      if (filter.fechaDesde) params = params.set('fechaDesde', filter.fechaDesde);
      if (filter.fechaHasta) params = params.set('fechaHasta', filter.fechaHasta);
      if (filter.search)     params = params.set('search',     filter.search);
    }
    return this.http.get<Incidencia[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Incidencia> {
    return this.http.get<Incidencia>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateIncidenciaDto): Observable<Incidencia> {
    return this.http.post<Incidencia>(this.baseUrl, dto);
  }

  update(id: number, dto: UpdateIncidenciaDto): Observable<Incidencia> {
    return this.http.put<Incidencia>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getComunidades(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/comunidades`);
  }

  getTipos(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/tipos`);
  }
}
