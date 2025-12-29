import { Routes } from '@angular/router';
import { ManagerDashboardComponent } from './manager-dashboard/manager-dashboard.component';
import { ManagerHomeComponent } from './manager-home/manager-home.component';
import { ProjectManagementComponent } from './project-management/project-management.component';
import { ProjectAssignmentsComponent } from './project-assignments/project-assignments.component';
import { TimesheetApprovalsComponent } from './timesheet-approvals/timesheet-approvals.component';
import { ReportsComponent } from './reports/reports.component';

export const MANAGER_ROUTES: Routes = [
  {
    path: '',
    component: ManagerDashboardComponent,
    children: [
      { path: '', component: ManagerHomeComponent },
      { path: 'projects', component: ProjectManagementComponent },
      { path: 'assignments', component: ProjectAssignmentsComponent },
      { path: 'approvals', component: TimesheetApprovalsComponent },
      { path: 'reports', component: ReportsComponent },
    ],
  },
];
