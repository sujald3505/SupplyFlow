export interface User {
  id: number;
  fullName: string;
  email: string;
  phone?: string | null;
  roleId: number;
  roleName: string;
  isActive: boolean;
}

export interface CreateUserRequest {
  fullName: string;
  email: string;
  password: string;
  phone?: string | null;
  roleId: number;
}

export interface UpdateUserRequest {
  fullName: string;
  phone?: string | null;
  roleId: number;
  isActive: boolean;
}