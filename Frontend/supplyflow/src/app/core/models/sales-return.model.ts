export interface CreateSalesReturnItemRequest {
  saleItemId: number;
  productId: number;
  quantity: number;
  unitPrice: number;
  discount: number;
  tax: number;
}

export interface CreateSalesReturnRequest {
  saleId: number;
  warehouseId: number;
  customerId: number;
  discount: number;
  tax: number;
  remarks?: string;
  items: CreateSalesReturnItemRequest[];
}

export interface SalesReturnItem {
  id: number;
  saleItemId: number;
  productId: number;
  productName: string;
  sku: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  tax: number;
  total: number;
}

export interface SalesReturn {
  id: number;
  returnNumber: string;
  saleId: number;
  invoiceNumber: string;

  warehouseId: number;
  warehouseName: string;

  customerId: number;
  customerName: string;

  returnDate: string;

  subTotal: number;
  discount: number;
  tax: number;
  grandTotal: number;

  status: string;
  remarks?: string | null;

  items: SalesReturnItem[];
}