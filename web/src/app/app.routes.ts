import { Routes } from '@angular/router';
import { authGuard, managerGuard, employeeGuard } from './core/guards';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: 'timesheet',
    loadChildren: () =>
      import('./features/timesheet/timesheet.routes').then((m) => m.TIMESHEET_ROUTES),
    canActivate: [authGuard, employeeGuard],
  },
  {
    path: 'manager',
    loadChildren: () => import('./features/manager/manager.routes').then((m) => m.MANAGER_ROUTES),
    canActivate: [authGuard, managerGuard],
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
