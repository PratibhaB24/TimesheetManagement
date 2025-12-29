import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService, UserService } from '../../../core/services';
import { User, UserRole } from '../../../core/models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private router = inject(Router);

  // Angular Signals for local UI state
  users = signal<User[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  managers = signal<User[]>([]);
  employees = signal<User[]>([]);

  constructor() {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users.set(users);
        this.managers.set(users.filter((u) => u.role === UserRole.Manager));
        this.employees.set(users.filter((u) => u.role === UserRole.Employee));
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load users. Make sure the API is running.');
        this.loading.set(false);
      },
    });
  }

  login(user: User): void {
    this.authService.login(user);

    if (user.role === UserRole.Manager) {
      this.router.navigate(['/manager']);
    } else {
      this.router.navigate(['/timesheet']);
    }
  }
}
