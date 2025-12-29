import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { User, UserRole } from '../../../core/models';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <header class="header">
      <div class="logo">
        <h1>⏱️ Timesheet</h1>
      </div>

      @if (user) {
      <nav class="nav-links">
        @if (user.role === UserRole.Employee) {
        <a routerLink="/timesheet" routerLinkActive="active">My Timesheets</a>
        } @if (user.role === UserRole.Manager) {
        <a routerLink="/manager" routerLinkActive="active">Dashboard</a>
        }
      </nav>

      <div class="user-info">
        <span class="name">{{ user.fullName }}</span>
        <span class="role">{{ getRoleName() }}</span>
        <button class="btn-logout" (click)="onLogout()">Logout</button>
      </div>
      }
    </header>
  `,
  styles: [
    `
      .header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 12px 24px;
        background: white;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.06);
        position: sticky;
        top: 0;
        z-index: 100;
      }

      .logo h1 {
        margin: 0;
        font-size: 20px;
        color: #667eea;
      }

      .nav-links {
        display: flex;
        gap: 24px;
      }

      .nav-links a {
        color: #4a5568;
        text-decoration: none;
        padding: 8px 16px;
        border-radius: 6px;
        transition: all 0.2s;
      }

      .nav-links a:hover {
        background: #f7fafc;
      }

      .nav-links a.active {
        background: #667eea;
        color: white;
      }

      .user-info {
        display: flex;
        align-items: center;
        gap: 12px;
      }

      .user-info .name {
        font-weight: 600;
        color: #1a202c;
      }

      .user-info .role {
        font-size: 12px;
        color: #718096;
        padding: 4px 8px;
        background: #e2e8f0;
        border-radius: 4px;
      }

      .btn-logout {
        padding: 8px 16px;
        background: #e53e3e;
        color: white;
        border: none;
        border-radius: 6px;
        cursor: pointer;
        font-size: 13px;
      }

      .btn-logout:hover {
        background: #c53030;
      }
    `,
  ],
})
export class HeaderComponent {
  @Input() user: User | null = null;
  @Output() logout = new EventEmitter<void>();

  UserRole = UserRole;

  getRoleName(): string {
    switch (this.user?.role) {
      case UserRole.Employee:
        return 'Employee';
      case UserRole.Manager:
        return 'Manager';
      default:
        return '';
    }
  }

  onLogout(): void {
    this.logout.emit();
  }
}
