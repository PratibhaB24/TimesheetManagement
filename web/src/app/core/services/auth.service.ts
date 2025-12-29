import { Injectable, signal, computed } from '@angular/core';
import { User, UserRole } from '../models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // Using Angular Signals for local UI state
  private currentUserSignal = signal<User | null>(null);

  // Computed signals for derived state
  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isLoggedIn = computed(() => this.currentUserSignal() !== null);
  readonly isManager = computed(() => this.currentUserSignal()?.role === UserRole.Manager);
  readonly isEmployee = computed(() => this.currentUserSignal()?.role === UserRole.Employee);
  readonly userRole = computed(() => this.currentUserSignal()?.role);

  constructor() {
    // Load user from localStorage on init
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      this.currentUserSignal.set(JSON.parse(storedUser));
    }
  }

  login(user: User): void {
    this.currentUserSignal.set(user);
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  logout(): void {
    this.currentUserSignal.set(null);
    localStorage.removeItem('currentUser');
  }

  hasRole(role: UserRole): boolean {
    return this.currentUserSignal()?.role === role;
  }
}
