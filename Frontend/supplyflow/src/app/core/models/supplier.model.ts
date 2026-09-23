export interface Supplier {
  id: number;
  name: string;
  contactPerson: string;
  email?: string;
  phone?: string;
  address?: string;
  gstNumber?: string;
  isActive: boolean;
}

export interface CreateSupplierRequest {
  name: string;
  contactPerson: string;
  email?: string;
  phone?: string;
  address?: string;
  gstNumber?: string;
}

export interface UpdateSupplierRequest {
  name: string;
  contactPerson: string;
  email?: string;
  phone?: string;
  address?: string;
  gstNumber?: string;
  isActive: boolean;
}