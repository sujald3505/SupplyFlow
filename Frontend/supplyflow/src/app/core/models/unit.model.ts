export interface Unit {
  id: number;
  name: string;
  symbol: string;
  description?: string;
  isActive: boolean;
}

export interface CreateUnitRequest {
  name: string;
  symbol: string;
  description?: string;
}

export interface UpdateUnitRequest {
  name: string;
  symbol: string;
  description?: string;
  isActive: boolean;
}