import { Component, Input, inject, signal, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { ProjectAssignmentService } from '../../../core/services';
import { ProjectAssignment, CreateTimesheetDto } from '../../../core/models';
import { createTimesheet } from '../../../store';

@Component({
  selector: 'app-timesheet-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './timesheet-form.component.html',
  styleUrl: './timesheet-form.component.scss',
})
export class TimesheetFormComponent implements OnInit, OnChanges {
  @Input() userId!: number;

  private fb = inject(FormBuilder);
  private store = inject(Store);
  private assignmentService = inject(ProjectAssignmentService);

  // Angular Signal for local UI state
  assignments = signal<ProjectAssignment[]>([]);
  submitting = signal(false);

  timesheetForm: FormGroup = this.fb.group({
    submissionDate: [new Date().toISOString().split('T')[0], Validators.required],
    entries: this.fb.array([]),
  });

  get entries(): FormArray {
    return this.timesheetForm.get('entries') as FormArray;
  }

  ngOnInit(): void {
    if (this.userId && this.userId > 0) {
      this.loadAssignments();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['userId'] && this.userId && this.userId > 0) {
      this.loadAssignments();
    }
  }

  private loadAssignments(): void {
    this.assignmentService.getByUser(this.userId).subscribe({
      next: (assignments) => this.assignments.set(assignments),
      error: (err) => console.error('Failed to load assignments', err),
    });
  }

  addEntry(): void {
    const entryGroup = this.fb.group({
      projectId: ['', Validators.required],
      date: [new Date().toISOString().split('T')[0], Validators.required],
      hours: [8, [Validators.required, Validators.min(0.25), Validators.max(24)]],
      description: ['', Validators.required],
    });

    this.entries.push(entryGroup);
  }

  removeEntry(index: number): void {
    this.entries.removeAt(index);
  }

  onSubmit(): void {
    if (this.timesheetForm.valid && this.entries.length > 0) {
      this.submitting.set(true);
      const formValue = this.timesheetForm.value;

      const timesheet: CreateTimesheetDto = {
        submissionDate: new Date(formValue.submissionDate),
        entries: formValue.entries.map((e: any) => ({
          projectId: parseInt(e.projectId),
          date: new Date(e.date),
          hours: e.hours,
          description: e.description,
        })),
      };

      this.store.dispatch(createTimesheet({ userId: this.userId, timesheet }));

      // Reset form
      this.timesheetForm.reset({
        submissionDate: new Date().toISOString().split('T')[0],
      });
      this.entries.clear();
      this.submitting.set(false);

      alert('Timesheet created successfully!');
    }
  }
}
