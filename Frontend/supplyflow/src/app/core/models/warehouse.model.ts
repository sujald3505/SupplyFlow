export interface Warehouse {
  id: number;
  name: string;
  code?: string;
  address?: string;
  contactPerson?: string;
  phone?: string;
  isActive: boolean;
}

export interface CreateWarehouseRequest {
  name: string;
  code?: string;
  address?: string;
  contactPerson?: string;
  phone?: string;
}

export interface UpdateWarehouseRequest {
  name: string;
  code?: string;
  address?: string;
  contactPerson?: string;
  phone?: string;
  isActive: boolean;
}