import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService } from '../../../core/services';
import {
  ReportFilter,
  EmployeeHoursSummary,
  ProjectHoursSummary,
  BillableReport,
} from '../../../core/models';

type ReportType = 'employee' | 'project' | 'billable';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss',
})
export class ReportsComponent {
  private reportService = inject(ReportService);

  // Local UI state with Angular Signals
  loading = signal(false);
  employeeReport = signal<EmployeeHoursSummary[]>([]);
  projectReport = signal<ProjectHoursSummary[]>([]);
  billableReport = signal<BillableReport | null>(null);

  selectedReport: ReportType = 'employee';
  startDate = this.getFirstDayOfMonth();
  endDate = this.getToday();

  private getFirstDayOfMonth(): string {
    const date = new Date();
    return new Date(date.getFullYear(), date.getMonth(), 1).toISOString().split('T')[0];
  }

  private getToday(): string {
    return new Date().toISOString().split('T')[0];
  }

  loadReport(): void {
    this.loading.set(true);

    const filter: ReportFilter = {
      startDate: new Date(this.startDate),
      endDate: new Date(this.endDate),
    };

    switch (this.selectedReport) {
      case 'employee':
        this.reportService.getEmployeeHoursSummary(filter).subscribe({
          next: (data) => {
            this.employeeReport.set(data);
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
        break;

      case 'project':
        this.reportService.getProjectHoursSummary(filter).subscribe({
          next: (data) => {
            this.projectReport.set(data);
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
        break;

      case 'billable':
        this.reportService.getBillableReport(filter).subscribe({
          next: (data) => {
            this.billableReport.set(data);
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
        break;
    }
  }
}
