import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Expediente,
  CreateExpedienteDto,
  UpdateExpedienteDto,
  ExpedienteFilter,
  TipoExpediente,
} from '../models/expediente.model';

@Injectable({ providedIn: 'root' })
export class ExpedienteService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/expediente`;

  getAll(filter?: ExpedienteFilter): Observable<Expediente[]> {
    let params = new HttpParams();
    if (filter) {
      if (filter.tipo)           params = params.set('tipo',           filter.tipo);
      if (filter.estado)         params = params.set('estado',         filter.estado);
      if (filter.comunidad)        params = params.set('comunidad',        filter.comunidad);
      if (filter.numeroComunidad) params = params.set('numeroComunidad', filter.numeroComunidad.toString());
      if (filter.tipoIncidencia) params = params.set('tipoIncidencia', filter.tipoIncidencia);
      if (filter.fechaDesde)     params = params.set('fechaDesde',     filter.fechaDesde);
      if (filter.fechaHasta)     params = params.set('fechaHasta',     filter.fechaHasta);
      if (filter.search)         params = params.set('search',         filter.search);
      if (filter.anio)            params = params.set('anio',            filter.anio.toString());
      if (filter.empresa)         params = params.set('empresa',         filter.empresa);
      if (filter.companiaSeguros) params = params.set('companiaSeguros', filter.companiaSeguros);
    }
    return this.http.get<Expediente[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Expediente> {
    return this.http.get<Expediente>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateExpedienteDto): Observable<Expediente> {
    return this.http.post<Expediente>(this.baseUrl, dto);
  }

  update(id: number, dto: UpdateExpedienteDto): Observable<Expediente> {
    return this.http.put<Expediente>(`${this.baseUrl}/${id}`, dto);
  }

  cambiarTipo(id: number, tipo: TipoExpediente): Observable<Expediente> {
    return this.http.patch<Expediente>(`${this.baseUrl}/${id}/tipo`, { tipo });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getComunidades(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/comunidades`);
  }

  getTiposIncidencia(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/tipos-incidencia`);
  }

  getAños(): Observable<number[]> {
    return this.http.get<number[]>(`${this.baseUrl}/anios`);
  }
}
