export interface EmployeeHoursSummary {
  userId: number;
  employeeName: string;
  totalHours: number;
  billableHours: number;
  nonBillableHours: number;
  approvedTimesheets: number;
}

export interface ProjectHoursSummary {
  projectId: number;
  projectCode: string;
  projectName: string;
  clientName: string;
  isBillable: boolean;
  totalHours: number;
  employeeCount: number;
}

export interface BillableDetail {
  projectCode: string;
  projectName: string;
  isBillable: boolean;
  hours: number;
}

export interface BillableReport {
  totalBillableHours: number;
  totalNonBillableHours: number;
  totalHours: number;
  billablePercentage: number;
  details: BillableDetail[];
}

export interface ReportFilter {
  startDate: Date;
  endDate: Date;
  userId?: number;
  projectId?: number;
}
