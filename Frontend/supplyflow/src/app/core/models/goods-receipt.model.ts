export interface GoodsReceiptItem {
  id: number;
  productId: number;
  productName: string;
  purchaseOrderItemId: number;

  receivedQuantity: number;
  acceptedQuantity?: number | null;
  rejectedQuantity?: number | null;

  remarks?: string | null;
}

export interface GoodsReceipt {
  id: number;
  grnNumber: string;

  purchaseOrderId: number;
  purchaseOrderNumber: string;

  warehouseId: number;
  warehouseName: string;

  receivedByUserId: number;
  receivedByUserName: string;

  status: string;

  receiptDate: string;
  remarks?: string | null;

  createdAt: string;

  items: GoodsReceiptItem[];
}


/* ==============================
   CREATE GOODS RECEIPT
================================ */

export interface CreateGoodsReceiptItem {
  productId: number;

  purchaseOrderItemId: number;

  receivedQuantity: number;

  acceptedQuantity?: number | null;

  rejectedQuantity?: number | null;

  remarks?: string | null;
}

export interface CreateGoodsReceipt {
  purchaseOrderId: number;

  receiptDate?: string | null;

  remarks?: string | null;

  items: CreateGoodsReceiptItem[];
}


/* ==============================
   GOODS RECEIPT STATUS
================================ */

export type GoodsReceiptStatus =
  | 'Draft'
  | 'Completed'
  | 'Cancelled';