import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Siniestro,
  CreateSiniestroDto,
  UpdateSiniestroDto,
  SiniestroFilter,
} from '../models/siniestro.model';

@Injectable({ providedIn: 'root' })
export class SiniestroService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/siniestro`;

  getAll(filter?: SiniestroFilter): Observable<Siniestro[]> {
    let params = new HttpParams();
    if (filter) {
      if (filter.comunidad)      params = params.set('comunidad',      filter.comunidad);
      if (filter.estado)         params = params.set('estado',         filter.estado);
      if (filter.companiaSeguros) params = params.set('companiaSeguros', filter.companiaSeguros);
      if (filter.fechaDesde)     params = params.set('fechaDesde',     filter.fechaDesde);
      if (filter.fechaHasta)     params = params.set('fechaHasta',     filter.fechaHasta);
      if (filter.search)         params = params.set('search',         filter.search);
    }
    return this.http.get<Siniestro[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Siniestro> {
    return this.http.get<Siniestro>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateSiniestroDto): Observable<Siniestro> {
    return this.http.post<Siniestro>(this.baseUrl, dto);
  }

  update(id: number, dto: UpdateSiniestroDto): Observable<Siniestro> {
    return this.http.put<Siniestro>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getComunidades(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/comunidades`);
  }

  getCompanias(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/companias`);
  }
}
