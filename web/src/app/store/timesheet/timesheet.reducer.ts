import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Timesheet, TimesheetStatus } from '../../core/models';
import * as TimesheetActions from './timesheet.actions';

export interface TimesheetState extends EntityState<Timesheet> {
  selectedTimesheetId: number | null;
  loading: boolean;
  error: string | null;
}

export const timesheetAdapter: EntityAdapter<Timesheet> = createEntityAdapter<Timesheet>({
  selectId: (timesheet: Timesheet) => timesheet.id,
  sortComparer: (a, b) =>
    new Date(b.submissionDate).getTime() - new Date(a.submissionDate).getTime(),
});

export const initialState: TimesheetState = timesheetAdapter.getInitialState({
  selectedTimesheetId: null,
  loading: false,
  error: null,
});

export const timesheetReducer = createReducer(
  initialState,

  // Load Timesheets
  on(TimesheetActions.loadUserTimesheets, TimesheetActions.loadPendingTimesheets, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(
    TimesheetActions.loadTimesheetsSuccess,
    TimesheetActions.loadPendingTimesheetsSuccess,
    (state, { timesheets }) => timesheetAdapter.setAll(timesheets, { ...state, loading: false })
  ),
  on(
    TimesheetActions.loadTimesheetsFailure,
    TimesheetActions.loadPendingTimesheetsFailure,
    (state, { error }) => ({
      ...state,
      loading: false,
      error,
    })
  ),

  // Create Timesheet
  on(TimesheetActions.createTimesheet, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(TimesheetActions.createTimesheetSuccess, (state, { timesheet }) =>
    timesheetAdapter.addOne(timesheet, { ...state, loading: false })
  ),
  on(TimesheetActions.createTimesheetFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Add Entry
  on(TimesheetActions.addTimesheetEntry, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(TimesheetActions.addTimesheetEntrySuccess, (state, { timesheet }) =>
    timesheetAdapter.upsertOne(timesheet, { ...state, loading: false })
  ),
  on(TimesheetActions.addTimesheetEntryFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Submit Timesheet
  on(TimesheetActions.submitTimesheet, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(TimesheetActions.submitTimesheetSuccess, (state, { timesheetId }) =>
    timesheetAdapter.updateOne(
      { id: timesheetId, changes: { status: TimesheetStatus.Submitted } },
      { ...state, loading: false }
    )
  ),
  on(TimesheetActions.submitTimesheetFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Approve Timesheet
  on(TimesheetActions.approveTimesheet, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(TimesheetActions.approveTimesheetSuccess, (state, { timesheetId }) =>
    timesheetAdapter.removeOne(timesheetId, { ...state, loading: false })
  ),
  on(TimesheetActions.approveTimesheetFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Reject Timesheet
  on(TimesheetActions.rejectTimesheet, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),
  on(TimesheetActions.rejectTimesheetSuccess, (state, { timesheetId }) =>
    timesheetAdapter.removeOne(timesheetId, { ...state, loading: false })
  ),
  on(TimesheetActions.rejectTimesheetFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  // Select Timesheet
  on(TimesheetActions.selectTimesheet, (state, { id }) => ({
    ...state,
    selectedTimesheetId: id,
  })),

  // Clear Error
  on(TimesheetActions.clearTimesheetError, (state) => ({
    ...state,
    error: null,
  }))
);
