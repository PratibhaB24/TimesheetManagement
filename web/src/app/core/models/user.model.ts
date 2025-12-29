export enum UserRole {
  Employee = 1,
  Manager = 2,
}

export interface User {
  id: number;
  email: string;
  fullName: string;
  role: UserRole;
  isActive: boolean;
  createdOn: Date;
}

export interface CreateUserDto {
  email: string;
  fullName: string;
  password: string;
  role: UserRole;
}

export interface UpdateUserDto {
  email: string;
  fullName: string;
  role: UserRole;
  isActive: boolean;
}
