export interface CreatePurchaseRequestItem {
  productId: number;

  requestedQuantity: number;

  remarks?: string;
}


export interface CreatePurchaseRequestRequest {
  remarks?: string;

  items: CreatePurchaseRequestItem[];
}


export interface PurchaseRequestItem {
  id: number;

  productId: number;

  productName: string;

  sku: string;

  requestedQuantity: number;

  unitName: string;

  unitSymbol: string;

  remarks?: string;
}


export interface PurchaseRequest {
  id: number;

  requestNumber: string;

  requestedByUserId: number;

  requestedByUserName: string;

  status: string;

  remarks?: string;

  submittedAt?: string;

  approvedAt?: string;

  approvedByUserId?: number;

  approvedByUserName?: string;

  rejectionReason?: string;

  createdAt: string;

  items: PurchaseRequestItem[];
}


export interface RejectPurchaseRequestRequest {
  rejectionReason: string;
}