import { createAction, props } from '@ngrx/store';
import { Project, CreateProjectDto, UpdateProjectDto } from '../../core/models';

// Load Projects
export const loadProjects = createAction('[Project] Load Projects');
export const loadProjectsSuccess = createAction(
  '[Project] Load Projects Success',
  props<{ projects: Project[] }>()
);
export const loadProjectsFailure = createAction(
  '[Project] Load Projects Failure',
  props<{ error: string }>()
);

// Create Project
export const createProject = createAction(
  '[Project] Create Project',
  props<{ project: CreateProjectDto }>()
);
export const createProjectSuccess = createAction(
  '[Project] Create Project Success',
  props<{ project: Project }>()
);
export const createProjectFailure = createAction(
  '[Project] Create Project Failure',
  props<{ error: string }>()
);

// Update Project
export const updateProject = createAction(
  '[Project] Update Project',
  props<{ id: number; project: UpdateProjectDto }>()
);
export const updateProjectSuccess = createAction(
  '[Project] Update Project Success',
  props<{ project: Project }>()
);
export const updateProjectFailure = createAction(
  '[Project] Update Project Failure',
  props<{ error: string }>()
);

// Activate/Deactivate Project
export const activateProject = createAction('[Project] Activate Project', props<{ id: number }>());
export const deactivateProject = createAction(
  '[Project] Deactivate Project',
  props<{ id: number }>()
);
export const toggleProjectStatusSuccess = createAction(
  '[Project] Toggle Status Success',
  props<{ id: number; isActive: boolean }>()
);
export const toggleProjectStatusFailure = createAction(
  '[Project] Toggle Status Failure',
  props<{ error: string }>()
);

// Delete Project
export const deleteProject = createAction('[Project] Delete Project', props<{ id: number }>());
export const deleteProjectSuccess = createAction(
  '[Project] Delete Project Success',
  props<{ id: number }>()
);
export const deleteProjectFailure = createAction(
  '[Project] Delete Project Failure',
  props<{ error: string }>()
);

// Select Project
export const selectProject = createAction(
  '[Project] Select Project',
  props<{ id: number | null }>()
);

// Clear Error
export const clearProjectError = createAction('[Project] Clear Error');
