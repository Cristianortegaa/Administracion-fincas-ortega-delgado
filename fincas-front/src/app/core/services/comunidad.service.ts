import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Comunidad, CreateComunidadDto, UpdateComunidadDto } from '../models/comunidad.model';

@Injectable({ providedIn: 'root' })
export class ComunidadService {
  private readonly http    = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/comunidades`;

  getAll(): Observable<Comunidad[]> {
    return this.http.get<Comunidad[]>(this.baseUrl);
  }

  getById(id: number): Observable<Comunidad> {
    return this.http.get<Comunidad>(`${this.baseUrl}/${id}`);
  }

  create(dto: CreateComunidadDto): Observable<Comunidad> {
    return this.http.post<Comunidad>(this.baseUrl, dto);
  }

  update(id: number, dto: UpdateComunidadDto): Observable<Comunidad> {
    return this.http.put<Comunidad>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
