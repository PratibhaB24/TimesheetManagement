import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Project, CreateProjectDto, UpdateProjectDto } from '../models';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  private apiUrl = `${environment.apiUrl}/projects`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Project[]> {
    return this.http
      .get<ApiResponse<Project[]>>(this.apiUrl)
      .pipe(map((response) => response.data));
  }

  getById(id: number): Observable<Project> {
    return this.http
      .get<ApiResponse<Project>>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response.data));
  }

  getActive(): Observable<Project[]> {
    return this.http
      .get<ApiResponse<Project[]>>(`${this.apiUrl}/active`)
      .pipe(map((response) => response.data));
  }

  create(project: CreateProjectDto): Observable<Project> {
    return this.http
      .post<ApiResponse<Project>>(this.apiUrl, project)
      .pipe(map((response) => response.data));
  }

  update(id: number, project: UpdateProjectDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, project);
  }

  activate(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/activate`, {});
  }

  deactivate(id: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/deactivate`, {});
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
