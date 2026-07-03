import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserDto, CreateUserDto, UpdateUserDto, ChangePasswordDto } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/user`;

  getAll(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(this.base);
  }

  create(dto: CreateUserDto): Observable<UserDto> {
    return this.http.post<UserDto>(this.base, dto);
  }

  update(id: number, dto: UpdateUserDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.base}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }

  changePassword(id: number, dto: ChangePasswordDto): Observable<void> {
    return this.http.put<void>(`${this.base}/${id}/password`, dto);
  }
}
