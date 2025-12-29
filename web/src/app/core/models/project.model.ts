export enum ProjectStatus {
  Active = 0,
  Inactive = 1,
  Completed = 2,
}

export interface Project {
  id: number;
  code: string;
  name: string;
  clientName: string;
  isBillable: boolean;
  status: ProjectStatus;
  createdOn: Date;
}

export interface CreateProjectDto {
  code: string;
  name: string;
  clientName: string;
  isBillable: boolean;
}

export interface UpdateProjectDto {
  code: string;
  name: string;
  clientName: string;
  isBillable: boolean;
  status: ProjectStatus;
}
