import { createAction, props } from '@ngrx/store';
import {
  Timesheet,
  CreateTimesheetDto,
  CreateTimesheetEntryDto,
  UpdateTimesheetEntryDto,
} from '../../core/models';

// Load Timesheets
export const loadUserTimesheets = createAction(
  '[Timesheet] Load User Timesheets',
  props<{ userId: number }>()
);
export const loadTimesheetsSuccess = createAction(
  '[Timesheet] Load Timesheets Success',
  props<{ timesheets: Timesheet[] }>()
);
export const loadTimesheetsFailure = createAction(
  '[Timesheet] Load Timesheets Failure',
  props<{ error: string }>()
);

// Load Pending Timesheets (Manager)
export const loadPendingTimesheets = createAction('[Timesheet] Load Pending Timesheets');
export const loadPendingTimesheetsSuccess = createAction(
  '[Timesheet] Load Pending Timesheets Success',
  props<{ timesheets: Timesheet[] }>()
);
export const loadPendingTimesheetsFailure = createAction(
  '[Timesheet] Load Pending Timesheets Failure',
  props<{ error: string }>()
);

// Create Timesheet
export const createTimesheet = createAction(
  '[Timesheet] Create Timesheet',
  props<{ userId: number; timesheet: CreateTimesheetDto }>()
);
export const createTimesheetSuccess = createAction(
  '[Timesheet] Create Timesheet Success',
  props<{ timesheet: Timesheet }>()
);
export const createTimesheetFailure = createAction(
  '[Timesheet] Create Timesheet Failure',
  props<{ error: string }>()
);

// Add Entry
export const addTimesheetEntry = createAction(
  '[Timesheet] Add Entry',
  props<{ timesheetId: number; entry: CreateTimesheetEntryDto }>()
);
export const addTimesheetEntrySuccess = createAction(
  '[Timesheet] Add Entry Success',
  props<{ timesheet: Timesheet }>()
);
export const addTimesheetEntryFailure = createAction(
  '[Timesheet] Add Entry Failure',
  props<{ error: string }>()
);

// Update Entry
export const updateTimesheetEntry = createAction(
  '[Timesheet] Update Entry',
  props<{ entryId: number; entry: UpdateTimesheetEntryDto }>()
);
export const updateTimesheetEntrySuccess = createAction('[Timesheet] Update Entry Success');
export const updateTimesheetEntryFailure = createAction(
  '[Timesheet] Update Entry Failure',
  props<{ error: string }>()
);

// Delete Entry
export const deleteTimesheetEntry = createAction(
  '[Timesheet] Delete Entry',
  props<{ entryId: number }>()
);
export const deleteTimesheetEntrySuccess = createAction(
  '[Timesheet] Delete Entry Success',
  props<{ entryId: number }>()
);
export const deleteTimesheetEntryFailure = createAction(
  '[Timesheet] Delete Entry Failure',
  props<{ error: string }>()
);

// Submit Timesheet
export const submitTimesheet = createAction('[Timesheet] Submit', props<{ timesheetId: number }>());
export const submitTimesheetSuccess = createAction(
  '[Timesheet] Submit Success',
  props<{ timesheetId: number }>()
);
export const submitTimesheetFailure = createAction(
  '[Timesheet] Submit Failure',
  props<{ error: string }>()
);

// Approve Timesheet
export const approveTimesheet = createAction(
  '[Timesheet] Approve',
  props<{ timesheetId: number }>()
);
export const approveTimesheetSuccess = createAction(
  '[Timesheet] Approve Success',
  props<{ timesheetId: number }>()
);
export const approveTimesheetFailure = createAction(
  '[Timesheet] Approve Failure',
  props<{ error: string }>()
);

// Reject Timesheet
export const rejectTimesheet = createAction(
  '[Timesheet] Reject',
  props<{ timesheetId: number; comments: string }>()
);
export const rejectTimesheetSuccess = createAction(
  '[Timesheet] Reject Success',
  props<{ timesheetId: number }>()
);
export const rejectTimesheetFailure = createAction(
  '[Timesheet] Reject Failure',
  props<{ error: string }>()
);

// Select Timesheet
export const selectTimesheet = createAction('[Timesheet] Select', props<{ id: number | null }>());

// Clear Error
export const clearTimesheetError = createAction('[Timesheet] Clear Error');
