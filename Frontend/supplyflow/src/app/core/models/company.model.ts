export interface Company {
  id: number;
  name: string;
  email: string;
  phone?: string | null;
  address?: string | null;
  gstNumber?: string | null;
  isActive: boolean;
}

export interface UpdateCompanyRequest {
  name: string;
  email: string;
  phone?: string | null;
  address?: string | null;
  gstNumber?: string | null;
}