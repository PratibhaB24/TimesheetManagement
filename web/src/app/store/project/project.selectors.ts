import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ProjectState, projectAdapter } from './project.reducer';
import { ProjectStatus } from '../../core/models';

export const selectProjectState = createFeatureSelector<ProjectState>('projects');

const { selectIds, selectEntities, selectAll, selectTotal } = projectAdapter.getSelectors();

export const selectAllProjects = createSelector(selectProjectState, selectAll);
export const selectProjectEntities = createSelector(selectProjectState, selectEntities);
export const selectProjectIds = createSelector(selectProjectState, selectIds);
export const selectProjectTotal = createSelector(selectProjectState, selectTotal);

export const selectProjectLoading = createSelector(selectProjectState, (state) => state.loading);

export const selectProjectError = createSelector(selectProjectState, (state) => state.error);

export const selectSelectedProjectId = createSelector(
  selectProjectState,
  (state) => state.selectedProjectId
);

export const selectSelectedProject = createSelector(
  selectProjectEntities,
  selectSelectedProjectId,
  (entities, selectedId) => (selectedId ? entities[selectedId] : null)
);

export const selectActiveProjects = createSelector(selectAllProjects, (projects) =>
  projects.filter((p) => p.status === ProjectStatus.Active)
);

export const selectBillableProjects = createSelector(selectAllProjects, (projects) =>
  projects.filter((p) => p.isBillable)
);
