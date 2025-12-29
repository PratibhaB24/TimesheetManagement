import { ProjectState, projectReducer } from './project/project.reducer';
import { TimesheetState, timesheetReducer } from './timesheet/timesheet.reducer';

export interface AppState {
  projects: ProjectState;
  timesheets: TimesheetState;
}

export const reducers = {
  projects: projectReducer,
  timesheets: timesheetReducer,
};

export { projectReducer, timesheetReducer };

// Project exports
export * from './project/project.actions';
export {
  selectAllProjects,
  selectProjectLoading,
  selectProjectError,
} from './project/project.selectors';
export { ProjectEffects } from './project/project.effects';

// Timesheet exports
export * from './timesheet/timesheet.actions';
export {
  selectAllTimesheets,
  selectTimesheetLoading,
  selectTimesheetError,
} from './timesheet/timesheet.selectors';
export { TimesheetEffects } from './timesheet/timesheet.effects';
