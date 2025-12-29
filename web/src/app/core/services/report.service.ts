import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  EmployeeHoursSummary,
  ProjectHoursSummary,
  BillableReport,
  ReportFilter,
} from '../models';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private apiUrl = `${environment.apiUrl}/reports`;

  constructor(private http: HttpClient) {}

  getEmployeeHoursSummary(filter: ReportFilter): Observable<EmployeeHoursSummary[]> {
    const params = this.buildParams(filter);
    return this.http
      .get<ApiResponse<EmployeeHoursSummary[]>>(`${this.apiUrl}/employee-hours`, { params })
      .pipe(map((response) => response.data));
  }

  getProjectHoursSummary(filter: ReportFilter): Observable<ProjectHoursSummary[]> {
    const params = this.buildParams(filter);
    return this.http
      .get<ApiResponse<ProjectHoursSummary[]>>(`${this.apiUrl}/project-hours`, { params })
      .pipe(map((response) => response.data));
  }

  getBillableReport(filter: ReportFilter): Observable<BillableReport> {
    const params = this.buildParams(filter);
    return this.http
      .get<ApiResponse<BillableReport>>(`${this.apiUrl}/billable`, { params })
      .pipe(map((response) => response.data));
  }

  private buildParams(filter: ReportFilter): HttpParams {
    let params = new HttpParams()
      .set('startDate', filter.startDate.toISOString())
      .set('endDate', filter.endDate.toISOString());

    if (filter.userId) {
      params = params.set('userId', filter.userId.toString());
    }
    if (filter.projectId) {
      params = params.set('projectId', filter.projectId.toString());
    }

    return params;
  }
}
