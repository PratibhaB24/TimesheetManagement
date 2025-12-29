import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  ProjectAssignment,
  CreateProjectAssignmentDto,
  UpdateProjectAssignmentDto,
} from '../models';

@Injectable({
  providedIn: 'root',
})
export class ProjectAssignmentService {
  private apiUrl = `${environment.apiUrl}/projectassignments`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ProjectAssignment[]> {
    return this.http
      .get<ApiResponse<ProjectAssignment[]>>(this.apiUrl)
      .pipe(map((response) => response.data));
  }

  getById(id: number): Observable<ProjectAssignment> {
    return this.http
      .get<ApiResponse<ProjectAssignment>>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response.data));
  }

  getByUser(userId: number): Observable<ProjectAssignment[]> {
    return this.http
      .get<ApiResponse<ProjectAssignment[]>>(`${this.apiUrl}/user/${userId}`)
      .pipe(map((response) => response.data));
  }

  getByProject(projectId: number): Observable<ProjectAssignment[]> {
    return this.http
      .get<ApiResponse<ProjectAssignment[]>>(`${this.apiUrl}/project/${projectId}`)
      .pipe(map((response) => response.data));
  }

  create(assignment: CreateProjectAssignmentDto): Observable<ProjectAssignment> {
    return this.http
      .post<ApiResponse<ProjectAssignment>>(this.apiUrl, assignment)
      .pipe(map((response) => response.data));
  }

  update(id: number, assignment: UpdateProjectAssignmentDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, assignment);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
