import { createFeatureSelector, createSelector } from '@ngrx/store';
import { TimesheetState, timesheetAdapter } from './timesheet.reducer';
import { TimesheetStatus } from '../../core/models';

export const selectTimesheetState = createFeatureSelector<TimesheetState>('timesheets');

const { selectIds, selectEntities, selectAll, selectTotal } = timesheetAdapter.getSelectors();

export const selectAllTimesheets = createSelector(selectTimesheetState, selectAll);
export const selectTimesheetEntities = createSelector(selectTimesheetState, selectEntities);
export const selectTimesheetIds = createSelector(selectTimesheetState, selectIds);
export const selectTimesheetTotal = createSelector(selectTimesheetState, selectTotal);

export const selectTimesheetLoading = createSelector(
  selectTimesheetState,
  (state) => state.loading
);

export const selectTimesheetError = createSelector(selectTimesheetState, (state) => state.error);

export const selectSelectedTimesheetId = createSelector(
  selectTimesheetState,
  (state) => state.selectedTimesheetId
);

export const selectSelectedTimesheet = createSelector(
  selectTimesheetEntities,
  selectSelectedTimesheetId,
  (entities, selectedId) => (selectedId ? entities[selectedId] : null)
);

export const selectDraftTimesheets = createSelector(selectAllTimesheets, (timesheets) =>
  timesheets.filter((t) => t.status === TimesheetStatus.Draft)
);

export const selectSubmittedTimesheets = createSelector(selectAllTimesheets, (timesheets) =>
  timesheets.filter((t) => t.status === TimesheetStatus.Submitted)
);

export const selectApprovedTimesheets = createSelector(selectAllTimesheets, (timesheets) =>
  timesheets.filter((t) => t.status === TimesheetStatus.Approved)
);

export const selectRejectedTimesheets = createSelector(selectAllTimesheets, (timesheets) =>
  timesheets.filter((t) => t.status === TimesheetStatus.Rejected)
);
