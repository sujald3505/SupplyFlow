// ========================================
// COMMON PAGINATION
// ========================================

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}


// ========================================
// INVENTORY STOCK REPORT
// ========================================

export interface InventoryStockReport {
  productId: number;
  productName: string;
  sku: string;
  categoryName: string;
  unitName: string;

  warehouseId: number;
  warehouseName: string;

  currentQuantity: number;
  minimumStockLevel: number;

  stockStatus: string;

  costPrice: number;
  inventoryValue: number;
}


export interface InventoryStockReportSummary {
  totalRecords: number;
  totalQuantity: number;
  totalInventoryValue: number;
  lowStockCount: number;
  outOfStockCount: number;
}


export interface InventoryStockReportResult {
  data: PagedResult<InventoryStockReport>;
  summary: InventoryStockReportSummary;
}


// ========================================
// INVENTORY TRANSACTION REPORT
// ========================================

export interface InventoryTransactionReport {
  id: number;
  createdAt: string;

  productId: number;
  productName: string;
  sku: string;

  warehouseId: number;
  warehouseName: string;

  transactionType: string;

  quantity: number;
  previousQuantity: number;
  newQuantity: number;

  referenceNumber?: string | null;
  remarks?: string | null;
}


export interface InventoryTransactionReportSummary {
  totalTransactions: number;
  totalTransactionQuantity: number;

  stockInTransactions: number;
  stockOutTransactions: number;
}


export interface InventoryTransactionReportResult {
  data: PagedResult<InventoryTransactionReport>;
  summary: InventoryTransactionReportSummary;
}


// ========================================
// LOW STOCK REPORT
// ========================================

export interface LowStockReport {
  productId: number;
  productName: string;
  sku: string;

  categoryName: string;

  warehouseId: number;
  warehouseName: string;

  currentQuantity: number;
  minimumStockLevel: number;
  shortageQuantity: number;
}


export interface LowStockReportSummary {
  totalLowStockProducts: number;
  totalShortageQuantity: number;
  outOfStockProducts: number;
}


export interface LowStockReportResult {
  data: PagedResult<LowStockReport>;
  summary: LowStockReportSummary;
}


// ========================================
// PURCHASE ORDER REPORT
// ========================================

export interface PurchaseOrderReport {
  id: number;

  purchaseOrderNumber: string;

  supplierId: number;
  supplierName: string;

  warehouseId: number;
  warehouseName: string;

  orderDate: string;
  expectedDeliveryDate?: string | null;

  status: string;

  totalAmount: number;

  createdAt: string;

  remarks?: string | null;
}


export interface PurchaseOrderReportSummary {
  totalPurchaseOrders: number;
  totalPurchaseAmount: number;

  draftCount: number;
  sentCount: number;
  confirmedCount: number;
  partiallyReceivedCount: number;
  completedCount: number;
  cancelledCount: number;
}


export interface PurchaseOrderReportResult {
  data: PagedResult<PurchaseOrderReport>;
  summary: PurchaseOrderReportSummary;
}


// ========================================
// QUERY MODELS
// ========================================

export interface BaseReportQuery {
  pageNumber?: number;
  pageSize?: number;
}


// Inventory Stock Query

export interface InventoryStockReportQuery
  extends BaseReportQuery {

  warehouseId?: number;
  productId?: number;
  categoryId?: number;
}


// Inventory Transaction Query

export interface InventoryTransactionReportQuery
  extends BaseReportQuery {

  warehouseId?: number;
  productId?: number;

  transactionType?: number;

  fromDate?: string;
  toDate?: string;
}


// Low Stock Query

export interface LowStockReportQuery
  extends BaseReportQuery {

  warehouseId?: number;
  productId?: number;
  categoryId?: number;
}


// Purchase Order Query

export interface PurchaseOrderReportQuery
  extends BaseReportQuery {

  supplierId?: number;
  warehouseId?: number;

  status?: number;

  fromDate?: string;
  toDate?: string;
}