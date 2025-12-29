import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Project, ProjectStatus } from '../../core/models';
import * as ProjectActions from './project.actions';

export interface ProjectState extends EntityState<Project> {
  selectedProjectId: number | null;
  loading: boolean;
  error: string | null;
}

export const projectAdapter: EntityAdapter<Project> = createEntityAdapter<Project>({
  selectId: (project: Project) => project.id,
  sortComparer: (a, b) => a.code.localeCompare(b.code),
});

export const initialState: ProjectState = projectAdapter.getInitialState({
  selectedProjectId: null,
  loading: false,
  error: null,
});

export const projectReducer = createReducer(
  initialState,

  // Load Projects
  on(ProjectActions.loadProjects, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(ProjectActions.loadProjectsSuccess, (state, { projects }) =>
    projectAdapter.setAll(projects, { ...state, loading: false })
  ),
  on(ProjectActions.loadProjectsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Create Project
  on(ProjectActions.createProject, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(ProjectActions.createProjectSuccess, (state, { project }) =>
    projectAdapter.addOne(project, { ...state, loading: false })
  ),
  on(ProjectActions.createProjectFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Update Project
  on(ProjectActions.updateProject, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(ProjectActions.updateProjectSuccess, (state, { project }) =>
    projectAdapter.upsertOne(project, { ...state, loading: false })
  ),
  on(ProjectActions.updateProjectFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Delete Project
  on(ProjectActions.deleteProject, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(ProjectActions.deleteProjectSuccess, (state, { id }) =>
    projectAdapter.removeOne(id, { ...state, loading: false })
  ),
  on(ProjectActions.deleteProjectFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Toggle Project Status (Activate/Deactivate)
  on(ProjectActions.toggleProjectStatusSuccess, (state, { id, isActive }) =>
    projectAdapter.updateOne(
      {
        id,
        changes: { status: isActive ? ProjectStatus.Active : ProjectStatus.Inactive },
      },
      { ...state, loading: false }
    )
  ),
  on(ProjectActions.toggleProjectStatusFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Select Project
  on(ProjectActions.selectProject, (state, { id }) => ({
    ...state,
    selectedProjectId: id,
  })),

  // Clear Error
  on(ProjectActions.clearProjectError, (state) => ({
    ...state,
    error: null,
  }))
);
