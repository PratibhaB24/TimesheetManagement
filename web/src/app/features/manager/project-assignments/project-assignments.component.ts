import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProjectAssignmentService, UserService, ProjectService } from '../../../core/services';
import { ProjectAssignment, User, Project, CreateProjectAssignmentDto } from '../../../core/models';

@Component({
  selector: 'app-project-assignments',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './project-assignments.component.html',
  styleUrl: './project-assignments.component.scss',
})
export class ProjectAssignmentsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private assignmentService = inject(ProjectAssignmentService);
  private userService = inject(UserService);
  private projectService = inject(ProjectService);

  // Local UI state with Angular Signals
  assignments = signal<ProjectAssignment[]>([]);
  employees = signal<User[]>([]);
  projects = signal<Project[]>([]);
  loading = signal(true);
  showForm = signal(false);

  assignmentForm: FormGroup = this.fb.group({
    userId: ['', Validators.required],
    projectId: ['', Validators.required],
    startDate: [new Date().toISOString().split('T')[0], Validators.required],
    endDate: [''],
  });

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.assignmentService.getAll().subscribe({
      next: (data) => {
        this.assignments.set(data);
        this.loading.set(false);
      },
    });

    this.userService.getEmployees().subscribe({
      next: (data) => this.employees.set(data),
    });

    this.projectService.getActive().subscribe({
      next: (data) => this.projects.set(data),
    });
  }

  onSubmit(): void {
    if (this.assignmentForm.valid) {
      const formValue = this.assignmentForm.value;

      const assignment: CreateProjectAssignmentDto = {
        userId: parseInt(formValue.userId),
        projectId: parseInt(formValue.projectId),
        startDate: new Date(formValue.startDate),
        endDate: formValue.endDate ? new Date(formValue.endDate) : undefined,
      };

      this.assignmentService.create(assignment).subscribe({
        next: (newAssignment) => {
          this.assignments.update((list) => [...list, newAssignment]);
          this.showForm.set(false);
          this.assignmentForm.reset({
            startDate: new Date().toISOString().split('T')[0],
          });
        },
      });
    }
  }

  deleteAssignment(id: number): void {
    if (confirm('Are you sure you want to delete this assignment?')) {
      this.assignmentService.delete(id).subscribe({
        next: () => {
          this.assignments.update((list) => list.filter((a) => a.id !== id));
        },
      });
    }
  }
}
