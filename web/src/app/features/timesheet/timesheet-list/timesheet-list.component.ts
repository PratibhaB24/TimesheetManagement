import { Component, Input, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { Timesheet, TimesheetStatus } from '../../../core/models';
import { submitTimesheet } from '../../../store';

@Component({
  selector: 'app-timesheet-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './timesheet-list.component.html',
  styleUrl: './timesheet-list.component.scss',
})
export class TimesheetListComponent {
  @Input() timesheets: Timesheet[] = [];
  @Input() loading = false;

  private store = inject(Store);

  TimesheetStatus = TimesheetStatus;
  selectedTimesheet = signal<Timesheet | null>(null);

  getStatusClass(status: TimesheetStatus): string {
    switch (status) {
      case TimesheetStatus.Draft:
        return 'draft';
      case TimesheetStatus.Submitted:
        return 'submitted';
      case TimesheetStatus.Approved:
        return 'approved';
      case TimesheetStatus.Rejected:
        return 'rejected';
      default:
        return '';
    }
  }

  getStatusText(status: TimesheetStatus): string {
    switch (status) {
      case TimesheetStatus.Draft:
        return 'Draft';
      case TimesheetStatus.Submitted:
        return 'Submitted';
      case TimesheetStatus.Approved:
        return 'Approved';
      case TimesheetStatus.Rejected:
        return 'Rejected';
      default:
        return '';
    }
  }

  submit(timesheetId: number): void {
    this.store.dispatch(submitTimesheet({ timesheetId }));
  }

  viewDetails(timesheet: Timesheet): void {
    this.selectedTimesheet.set(timesheet);
  }

  closeDetails(): void {
    this.selectedTimesheet.set(null);
  }
}
