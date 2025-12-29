import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './core/services';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  template: ` <router-outlet /> `,
  styles: [
    `
      :host {
        display: block;
        min-height: 100vh;
        background: #f7fafc;
      }
    `,
  ],
})
export class App {
  private authService = inject(AuthService);
  currentUser = this.authService.currentUser;
}
