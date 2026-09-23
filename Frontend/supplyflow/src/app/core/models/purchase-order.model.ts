export interface PurchaseOrderItem {
  id: number;

  productId: number;

  productName: string;

  sku: string;

  orderedQuantity: number;

  unitPrice: number;

  totalPrice: number;

  receivedQuantity: number;

  remainingQuantity: number;
}


export interface PurchaseOrder {
  id: number;

  purchaseOrderNumber: string;

  supplierId: number;

  supplierName: string;

  warehouseId: number;

  warehouseName: string;

  purchaseRequestId?: number;

  purchaseRequestNumber?: string;

  status: string;

  orderDate: string;

  expectedDeliveryDate?: string;

  totalAmount: number;

  remarks?: string;

  createdAt: string;

  items: PurchaseOrderItem[];
}


export interface CreatePurchaseOrderItem {
  productId: number;

  orderedQuantity: number;

  unitPrice: number;
}


export interface CreatePurchaseOrder {
  supplierId: number;

  warehouseId: number;

  purchaseRequestId?: number;

  expectedDeliveryDate?: string;

  remarks?: string;

  items: CreatePurchaseOrderItem[];
}