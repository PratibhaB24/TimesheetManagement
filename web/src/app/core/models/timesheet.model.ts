export enum TimesheetStatus {
  Draft = 1,
  Submitted = 2,
  Approved = 3,
  Rejected = 4,
}

export interface TimesheetEntry {
  id: number;
  timesheetId: number;
  projectId: number;
  projectCode: string;
  projectName: string;
  date: Date;
  hours: number;
  description: string;
}

export interface Timesheet {
  id: number;
  userId: number;
  userName: string;
  submissionDate: Date;
  status: TimesheetStatus;
  totalHours: number;
  rejectionComments?: string;
  entries: TimesheetEntry[];
  createdOn: Date;
}

export interface CreateTimesheetEntryDto {
  projectId: number;
  date: Date;
  hours: number;
  description: string;
}

export interface CreateTimesheetDto {
  submissionDate: Date;
  entries: CreateTimesheetEntryDto[];
}

export interface UpdateTimesheetEntryDto {
  hours: number;
  description: string;
}
