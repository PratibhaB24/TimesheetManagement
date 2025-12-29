import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  Timesheet,
  CreateTimesheetDto,
  CreateTimesheetEntryDto,
  UpdateTimesheetEntryDto,
} from '../models';

@Injectable({
  providedIn: 'root',
})
export class TimesheetService {
  private apiUrl = `${environment.apiUrl}/timesheets`;

  constructor(private http: HttpClient) {}

  getById(id: number): Observable<Timesheet> {
    return this.http
      .get<ApiResponse<Timesheet>>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response.data));
  }

  getUserTimesheets(userId: number): Observable<Timesheet[]> {
    return this.http
      .get<ApiResponse<Timesheet[]>>(`${this.apiUrl}/user/${userId}`)
      .pipe(map((response) => response.data));
  }

  getPending(): Observable<Timesheet[]> {
    return this.http
      .get<ApiResponse<Timesheet[]>>(`${this.apiUrl}/pending`)
      .pipe(map((response) => response.data));
  }

  create(userId: number, timesheet: CreateTimesheetDto): Observable<Timesheet> {
    return this.http
      .post<ApiResponse<Timesheet>>(`${this.apiUrl}?userId=${userId}`, timesheet)
      .pipe(map((response) => response.data));
  }

  addEntry(timesheetId: number, entry: CreateTimesheetEntryDto): Observable<Timesheet> {
    return this.http
      .post<ApiResponse<Timesheet>>(`${this.apiUrl}/${timesheetId}/entries`, entry)
      .pipe(map((response) => response.data));
  }

  updateEntry(entryId: number, entry: UpdateTimesheetEntryDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/entries/${entryId}`, entry);
  }

  deleteEntry(entryId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/entries/${entryId}`);
  }

  submit(timesheetId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${timesheetId}/submit`, {});
  }

  approve(timesheetId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${timesheetId}/approve`, {});
  }

  reject(timesheetId: number, comments: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${timesheetId}/reject`, {
      rejectionComments: comments,
    });
  }
}
