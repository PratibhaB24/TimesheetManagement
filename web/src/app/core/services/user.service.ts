import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, User, CreateUserDto, UpdateUserDto } from '../models';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<User[]> {
    return this.http.get<ApiResponse<User[]>>(this.apiUrl).pipe(map((response) => response.data));
  }

  getById(id: number): Observable<User> {
    return this.http
      .get<ApiResponse<User>>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response.data));
  }

  getEmployees(): Observable<User[]> {
    return this.http
      .get<ApiResponse<User[]>>(`${this.apiUrl}/employees`)
      .pipe(map((response) => response.data));
  }

  getManagers(): Observable<User[]> {
    return this.http
      .get<ApiResponse<User[]>>(`${this.apiUrl}/managers`)
      .pipe(map((response) => response.data));
  }

  create(user: CreateUserDto): Observable<User> {
    return this.http
      .post<ApiResponse<User>>(this.apiUrl, user)
      .pipe(map((response) => response.data));
  }

  update(id: number, user: UpdateUserDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, user);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
