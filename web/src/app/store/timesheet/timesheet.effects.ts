import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError, switchMap } from 'rxjs/operators';
import { TimesheetService } from '../../core/services';
import * as TimesheetActions from './timesheet.actions';

@Injectable()
export class TimesheetEffects {
  private actions$ = inject(Actions);
  private timesheetService = inject(TimesheetService);

  loadUserTimesheets$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.loadUserTimesheets),
      switchMap(({ userId }) =>
        this.timesheetService.getUserTimesheets(userId).pipe(
          map((timesheets) => TimesheetActions.loadTimesheetsSuccess({ timesheets })),
          catchError((error) =>
            of(TimesheetActions.loadTimesheetsFailure({ error: error.message }))
          )
        )
      )
    )
  );

  loadPendingTimesheets$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.loadPendingTimesheets),
      switchMap(() =>
        this.timesheetService.getPending().pipe(
          map((timesheets) => TimesheetActions.loadPendingTimesheetsSuccess({ timesheets })),
          catchError((error) =>
            of(TimesheetActions.loadPendingTimesheetsFailure({ error: error.message }))
          )
        )
      )
    )
  );

  createTimesheet$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.createTimesheet),
      mergeMap(({ userId, timesheet }) =>
        this.timesheetService.create(userId, timesheet).pipe(
          map((newTimesheet) =>
            TimesheetActions.createTimesheetSuccess({ timesheet: newTimesheet })
          ),
          catchError((error) =>
            of(TimesheetActions.createTimesheetFailure({ error: error.message }))
          )
        )
      )
    )
  );

  addEntry$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.addTimesheetEntry),
      mergeMap(({ timesheetId, entry }) =>
        this.timesheetService.addEntry(timesheetId, entry).pipe(
          map((timesheet) => TimesheetActions.addTimesheetEntrySuccess({ timesheet })),
          catchError((error) =>
            of(TimesheetActions.addTimesheetEntryFailure({ error: error.message }))
          )
        )
      )
    )
  );

  updateEntry$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.updateTimesheetEntry),
      mergeMap(({ entryId, entry }) =>
        this.timesheetService.updateEntry(entryId, entry).pipe(
          map(() => TimesheetActions.updateTimesheetEntrySuccess()),
          catchError((error) =>
            of(TimesheetActions.updateTimesheetEntryFailure({ error: error.message }))
          )
        )
      )
    )
  );

  deleteEntry$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.deleteTimesheetEntry),
      mergeMap(({ entryId }) =>
        this.timesheetService.deleteEntry(entryId).pipe(
          map(() => TimesheetActions.deleteTimesheetEntrySuccess({ entryId })),
          catchError((error) =>
            of(TimesheetActions.deleteTimesheetEntryFailure({ error: error.message }))
          )
        )
      )
    )
  );

  submitTimesheet$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.submitTimesheet),
      mergeMap(({ timesheetId }) =>
        this.timesheetService.submit(timesheetId).pipe(
          map(() => TimesheetActions.submitTimesheetSuccess({ timesheetId })),
          catchError((error) =>
            of(TimesheetActions.submitTimesheetFailure({ error: error.message }))
          )
        )
      )
    )
  );

  approveTimesheet$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.approveTimesheet),
      mergeMap(({ timesheetId }) =>
        this.timesheetService.approve(timesheetId).pipe(
          map(() => TimesheetActions.approveTimesheetSuccess({ timesheetId })),
          catchError((error) =>
            of(TimesheetActions.approveTimesheetFailure({ error: error.message }))
          )
        )
      )
    )
  );

  rejectTimesheet$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TimesheetActions.rejectTimesheet),
      mergeMap(({ timesheetId, comments }) =>
        this.timesheetService.reject(timesheetId, comments).pipe(
          map(() => TimesheetActions.rejectTimesheetSuccess({ timesheetId })),
          catchError((error) =>
            of(TimesheetActions.rejectTimesheetFailure({ error: error.message }))
          )
        )
      )
    )
  );
}
