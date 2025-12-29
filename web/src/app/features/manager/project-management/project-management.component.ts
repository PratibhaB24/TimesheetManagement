import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Project, ProjectStatus, CreateProjectDto, UpdateProjectDto } from '../../../core/models';
import {
  loadProjects,
  createProject,
  activateProject,
  deactivateProject,
  selectAllProjects,
  selectProjectLoading,
} from '../../../store';

@Component({
  selector: 'app-project-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './project-management.component.html',
  styleUrl: './project-management.component.scss',
})
export class ProjectManagementComponent implements OnInit {
  private store = inject(Store);
  private fb = inject(FormBuilder);

  projects = this.store.selectSignal(selectAllProjects);
  loading = this.store.selectSignal(selectProjectLoading);

  // Local UI state with Angular Signals
  showForm = signal(false);

  ProjectStatus = ProjectStatus;

  projectForm: FormGroup = this.fb.group({
    code: ['', [Validators.required, Validators.maxLength(20)]],
    name: ['', [Validators.required, Validators.maxLength(100)]],
    clientName: ['', [Validators.required, Validators.maxLength(100)]],
    isBillable: [true],
  });

  ngOnInit(): void {
    this.store.dispatch(loadProjects());
  }

  getStatusClass(status: ProjectStatus): string {
    switch (status) {
      case ProjectStatus.Active:
        return 'active';
      case ProjectStatus.Inactive:
        return 'inactive';
      case ProjectStatus.Completed:
        return 'completed';
      default:
        return '';
    }
  }

  getStatusText(status: ProjectStatus): string {
    switch (status) {
      case ProjectStatus.Active:
        return 'Active';
      case ProjectStatus.Inactive:
        return 'Inactive';
      case ProjectStatus.Completed:
        return 'Completed';
      default:
        return '';
    }
  }

  onSubmit(): void {
    if (this.projectForm.valid) {
      const project: CreateProjectDto = this.projectForm.value;
      this.store.dispatch(createProject({ project }));
      this.showForm.set(false);
      this.projectForm.reset({ isBillable: true });
    }
  }

  activate(id: number): void {
    this.store.dispatch(activateProject({ id }));
  }

  deactivate(id: number): void {
    this.store.dispatch(deactivateProject({ id }));
  }
}
