import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { loadPendingTimesheets, selectAllTimesheets, selectTimesheetLoading } from '../../../store';
import { ReportService } from '../../../core/services';
import { ReportFilter, EmployeeHoursSummary } from '../../../core/models';

@Component({
  selector: 'app-manager-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './manager-home.component.html',
  styleUrl: './manager-home.component.scss',
})
export class ManagerHomeComponent implements OnInit {
  private store = inject(Store);
  private reportService = inject(ReportService);

  timesheets = this.store.selectSignal(selectAllTimesheets);
  loading = this.store.selectSignal(selectTimesheetLoading);

  // Local UI state with signals
  pendingCount = signal(0);
  totalHoursThisMonth = signal(0);
  activeEmployees = signal(0);

  ngOnInit(): void {
    this.store.dispatch(loadPendingTimesheets());
    this.loadStats();
  }

  private loadStats(): void {
    const now = new Date();
    const startOfMonth = new Date(now.getFullYear(), now.getMonth(), 1);

    const filter: ReportFilter = {
      startDate: startOfMonth,
      endDate: now,
    };

    this.reportService.getEmployeeHoursSummary(filter).subscribe({
      next: (data) => {
        this.totalHoursThisMonth.set(data.reduce((sum, e) => sum + e.totalHours, 0));
        this.activeEmployees.set(data.length);
      },
    });
  }
}
