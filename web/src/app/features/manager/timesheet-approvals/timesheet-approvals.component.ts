import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Timesheet, TimesheetStatus } from '../../../core/models';
import {
  loadPendingTimesheets,
  approveTimesheet,
  rejectTimesheet,
  selectAllTimesheets,
  selectTimesheetLoading,
} from '../../../store';

@Component({
  selector: 'app-timesheet-approvals',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './timesheet-approvals.component.html',
  styleUrl: './timesheet-approvals.component.scss',
})
export class TimesheetApprovalsComponent implements OnInit {
  private store = inject(Store);

  timesheets = this.store.selectSignal(selectAllTimesheets);
  loading = this.store.selectSignal(selectTimesheetLoading);

  // Local UI state with Angular Signals
  showRejectModal = signal(false);
  selectedTimesheetId = signal<number | null>(null);
  rejectionComments = '';

  ngOnInit(): void {
    this.store.dispatch(loadPendingTimesheets());
  }

  approve(timesheetId: number): void {
    this.store.dispatch(approveTimesheet({ timesheetId }));
  }

  openRejectModal(timesheet: Timesheet): void {
    this.selectedTimesheetId.set(timesheet.id);
    this.rejectionComments = '';
    this.showRejectModal.set(true);
  }

  closeRejectModal(): void {
    this.showRejectModal.set(false);
    this.selectedTimesheetId.set(null);
    this.rejectionComments = '';
  }

  confirmReject(): void {
    const timesheetId = this.selectedTimesheetId();
    if (timesheetId && this.rejectionComments.length >= 10) {
      this.store.dispatch(
        rejectTimesheet({
          timesheetId,
          comments: this.rejectionComments,
        })
      );
      this.closeRejectModal();
    }
  }
}
