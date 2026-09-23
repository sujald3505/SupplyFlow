export interface CreateSaleItemRequest {
  productId: number;
  quantity: number;
  unitPrice: number;
  discount: number;
  tax: number;
}

export interface CreateSaleRequest {
  warehouseId: number;
  customerId: number;
  discount: number;
  tax: number;
  remarks?: string;
  items: CreateSaleItemRequest[];
}

export interface SaleItem {
  id: number;
  productId: number;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  tax: number;
  total: number;
}

export interface Sale {
  id: number;
  invoiceNumber: string;
  warehouseId: number;
  warehouseName: string;
  customerId: number;
  saleDate: string;
  subTotal: number;
  discount: number;
  tax: number;
  grandTotal: number;
  status: string;
  remarks?: string;
  items: SaleItem[];
}