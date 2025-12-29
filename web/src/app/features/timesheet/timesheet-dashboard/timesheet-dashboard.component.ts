import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { AuthService } from '../../../core/services';
import { TimesheetStatus } from '../../../core/models';
import { loadUserTimesheets, selectAllTimesheets, selectTimesheetLoading } from '../../../store';
import { TimesheetFormComponent } from '../timesheet-form/timesheet-form.component';
import { TimesheetListComponent } from '../timesheet-list/timesheet-list.component';

@Component({
  selector: 'app-timesheet-dashboard',
  standalone: true,
  imports: [CommonModule, TimesheetFormComponent, TimesheetListComponent],
  templateUrl: './timesheet-dashboard.component.html',
  styleUrl: './timesheet-dashboard.component.scss',
})
export class TimesheetDashboardComponent implements OnInit {
  private store = inject(Store);
  private authService = inject(AuthService);

  currentUser = this.authService.currentUser;
  timesheets = this.store.selectSignal(selectAllTimesheets);
  loading = this.store.selectSignal(selectTimesheetLoading);

  // Computed signals for stats
  draftCount = computed(
    () => this.timesheets().filter((t) => t.status === TimesheetStatus.Draft).length
  );
  submittedCount = computed(
    () => this.timesheets().filter((t) => t.status === TimesheetStatus.Submitted).length
  );
  approvedCount = computed(
    () => this.timesheets().filter((t) => t.status === TimesheetStatus.Approved).length
  );
  rejectedCount = computed(
    () => this.timesheets().filter((t) => t.status === TimesheetStatus.Rejected).length
  );

  ngOnInit(): void {
    const userId = this.currentUser()?.id;
    if (userId) {
      this.store.dispatch(loadUserTimesheets({ userId }));
    }
  }

  logout(): void {
    this.authService.logout();
    window.location.href = '/login';
  }
}
