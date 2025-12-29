import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="manager-layout">
      <aside class="sidebar">
        <div class="logo">
          <h2>Timesheet</h2>
          <span>Manager Portal</span>
        </div>

        <nav class="nav-menu">
          <a
            routerLink="/manager"
            routerLinkActive="active"
            [routerLinkActiveOptions]="{ exact: true }"
          >
            <span class="icon">📊</span> Dashboard
          </a>
          <a routerLink="/manager/projects" routerLinkActive="active">
            <span class="icon">📁</span> Projects
          </a>
          <a routerLink="/manager/assignments" routerLinkActive="active">
            <span class="icon">👥</span> Assignments
          </a>
          <a routerLink="/manager/approvals" routerLinkActive="active">
            <span class="icon">✅</span> Approvals
          </a>
          <a routerLink="/manager/reports" routerLinkActive="active">
            <span class="icon">📈</span> Reports
          </a>
        </nav>

        <div class="user-section">
          <div class="user-info">
            <span class="name">{{ currentUser()?.fullName }}</span>
            <span class="role">Manager</span>
          </div>
          <button class="btn-logout" (click)="logout()">Logout</button>
        </div>
      </aside>

      <main class="main-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [
    `
      .manager-layout {
        display: flex;
        min-height: 100vh;
      }

      .sidebar {
        width: 250px;
        background: #1a202c;
        color: white;
        display: flex;
        flex-direction: column;
        position: fixed;
        height: 100vh;
      }

      .logo {
        padding: 24px;
        border-bottom: 1px solid #2d3748;
      }

      .logo h2 {
        margin: 0;
        font-size: 20px;
      }

      .logo span {
        font-size: 12px;
        color: #a0aec0;
      }

      .nav-menu {
        flex: 1;
        padding: 16px 0;
      }

      .nav-menu a {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px 24px;
        color: #a0aec0;
        text-decoration: none;
        transition: all 0.2s;
      }

      .nav-menu a:hover {
        background: #2d3748;
        color: white;
      }

      .nav-menu a.active {
        background: #667eea;
        color: white;
      }

      .icon {
        font-size: 18px;
      }

      .user-section {
        padding: 16px 24px;
        border-top: 1px solid #2d3748;
      }

      .user-info {
        display: flex;
        flex-direction: column;
        margin-bottom: 12px;
      }

      .user-info .name {
        font-weight: 600;
      }

      .user-info .role {
        font-size: 12px;
        color: #a0aec0;
      }

      .btn-logout {
        width: 100%;
        padding: 8px;
        background: #e53e3e;
        color: white;
        border: none;
        border-radius: 6px;
        cursor: pointer;
      }

      .btn-logout:hover {
        background: #c53030;
      }

      .main-content {
        flex: 1;
        margin-left: 250px;
        background: #f7fafc;
        padding: 24px;
      }
    `,
  ],
})
export class ManagerDashboardComponent {
  private authService = inject(AuthService);
  currentUser = this.authService.currentUser;

  logout(): void {
    this.authService.logout();
    window.location.href = '/login';
  }
}
