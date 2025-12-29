import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-error-display',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="error-container" [class]="type">
      <span class="icon">
        @switch (type) { @case ('error') { ❌ } @case ('warning') { ⚠️ } @case ('info') { ℹ️ }
        @default { ❌ } }
      </span>
      <div class="content">
        @if (title) {
        <strong>{{ title }}</strong>
        }
        <p>{{ message }}</p>
      </div>
    </div>
  `,
  styles: [
    `
      .error-container {
        display: flex;
        gap: 12px;
        padding: 16px;
        border-radius: 8px;
        margin: 16px 0;
      }

      .error-container.error {
        background: #fff5f5;
        border: 1px solid #fc8181;
        color: #c53030;
      }

      .error-container.warning {
        background: #fffaf0;
        border: 1px solid #f6ad55;
        color: #c05621;
      }

      .error-container.info {
        background: #ebf8ff;
        border: 1px solid #63b3ed;
        color: #2b6cb0;
      }

      .icon {
        font-size: 20px;
      }

      .content strong {
        display: block;
        margin-bottom: 4px;
      }

      .content p {
        margin: 0;
        font-size: 14px;
      }
    `,
  ],
})
export class ErrorDisplayComponent {
  @Input() message = 'An error occurred';
  @Input() title?: string;
  @Input() type: 'error' | 'warning' | 'info' = 'error';
}
