export enum InventoryTransactionType {
  StockIn = 1,
  StockOut = 2,
  Adjustment = 3,
}


export interface InventoryTransaction {
  id: number;

  warehouseId: number;
  warehouseName: string;

  productId: number;
  productName: string;

  sku: string;

  transactionType: string;

  quantity: number;

  previousQuantity: number;
  newQuantity: number;

  referenceNumber?: string;

  remarks?: string;

  createdAt: string;
}


export interface CreateInventoryTransactionRequest {
  warehouseId: number;

  productId: number;

  transactionType: InventoryTransactionType;

  quantity: number;

  referenceNumber?: string;

  remarks?: string;
}