import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="loading-overlay" [class.inline]="inline">
      <div class="spinner"></div>
      @if (message) {
      <p class="message">{{ message }}</p>
      }
    </div>
  `,
  styles: [
    `
      .loading-overlay {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        padding: 40px;
      }

      .loading-overlay:not(.inline) {
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(255, 255, 255, 0.9);
        z-index: 1000;
      }

      .spinner {
        width: 40px;
        height: 40px;
        border: 3px solid #e2e8f0;
        border-top-color: #667eea;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
      }

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
      }

      .message {
        margin-top: 16px;
        color: #4a5568;
        font-size: 14px;
      }
    `,
  ],
})
export class LoadingSpinnerComponent {
  @Input() message?: string;
  @Input() inline = true;
}
