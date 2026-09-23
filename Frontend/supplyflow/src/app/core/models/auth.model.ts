export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiration: string;
  fullName: string;
  email: string;
  role: string;
  userId: number;
  companyId: number;
}

export interface RegisterRequest {
  companyId: number;
  roleId: number;
  fullName: string;
  email: string;
  password: string;
  phone?: string | null;
}

export interface RegisterResponse {
  userId: number;
  companyId: number;
  roleId: number;
  fullName: string;
  email: string;
  phone?: string | null;
  role: string;
  isActive: boolean;
}