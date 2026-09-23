export interface DashboardSummary {
  totalProducts: number;
  totalSuppliers: number;
  totalWarehouses: number;
  lowStockProducts: number;
}

export interface RecentPurchaseOrder {
  id: number;
  purchaseOrderNumber: string;
  supplierName: string;
  totalAmount: number;
  status: string;
  orderDate: string;
}

export interface RecentPurchaseRequest {
  id: number;
  requestNumber: string;
  requestedBy: string;
  status: string;
  requestedDate: string;
}

export interface RecentGoodsReceipt {
  id: number;
  grnNumber: string;
  purchaseOrderNumber: string;
  receivedBy: string;
  receiptDate: string;
}

export interface DashboardData {
  summary: DashboardSummary;
  recentPurchaseOrders: RecentPurchaseOrder[];
  recentPurchaseRequests: RecentPurchaseRequest[];
  recentGoodsReceipts: RecentGoodsReceipt[];
}