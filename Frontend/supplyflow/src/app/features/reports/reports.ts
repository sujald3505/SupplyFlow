import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ReportService } from '../../core/services/report.service';

import {
  InventoryStockReport,
  InventoryTransactionReport,
  LowStockReport,
  PurchaseOrderReport,
} from '../../core/models/report.model';

type ReportTab = 'inventory-stock' | 'inventory-transactions' | 'low-stock' | 'purchase-orders';

@Component({
  selector: 'app-reports',
  standalone: true,

  imports: [CommonModule, FormsModule],

  templateUrl: './reports.html',
})
export class Reports implements OnInit {
  // ========================================
  // TAB
  // ========================================

  activeTab: ReportTab = 'inventory-stock';

  // ========================================
  // LOADING
  // ========================================

  loading = false;

  // ========================================
  // ERROR
  // ========================================

  errorMessage = '';

  // ========================================
  // INVENTORY STOCK
  // ========================================

  inventoryStockData: InventoryStockReport[] = [];

  inventoryStockSummary = {
    totalRecords: 0,
    totalQuantity: 0,
    totalInventoryValue: 0,
    lowStockCount: 0,
    outOfStockCount: 0,
  };

  // ========================================
  // INVENTORY TRANSACTIONS
  // ========================================

  inventoryTransactionData: InventoryTransactionReport[] = [];

  inventoryTransactionSummary = {
    totalTransactions: 0,
    totalTransactionQuantity: 0,
    stockInTransactions: 0,
    stockOutTransactions: 0,
  };

  // ========================================
  // LOW STOCK
  // ========================================

  lowStockData: LowStockReport[] = [];

  lowStockSummary = {
    totalLowStockProducts: 0,
    totalShortageQuantity: 0,
    outOfStockProducts: 0,
  };

  // ========================================
  // PURCHASE ORDERS
  // ========================================

  purchaseOrderData: PurchaseOrderReport[] = [];

  purchaseOrderSummary = {
    totalPurchaseOrders: 0,
    totalPurchaseAmount: 0,

    draftCount: 0,
    sentCount: 0,
    confirmedCount: 0,
    partiallyReceivedCount: 0,
    completedCount: 0,
    cancelledCount: 0,
  };

  // ========================================
  // FILTERS
  // ========================================

  warehouseId?: number;

  productId?: number;

  categoryId?: number;

  supplierId?: number;

  transactionType?: number;

  purchaseOrderStatus?: number;

  fromDate = '';

  toDate = '';

  // ========================================
  // PAGINATION
  // ========================================

  pageNumber = 1;

  pageSize = 10;

  totalCount = 0;

  totalPages = 0;

  constructor(
    private readonly reportService: ReportService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  // ========================================
  // INIT
  // ========================================

  ngOnInit(): void {
    this.loadDashboard();
  }

  // ========================================
  // DASHBOARD
  // ========================================

  loadDashboard(): void {
    this.loading = true;

    this.errorMessage = '';

    this.loadInventoryStock();

    this.loadInventoryTransactions();

    this.loadLowStock();

    this.loadPurchaseOrders();
  }

  // ========================================
  // TAB CHANGE
  // ========================================

  changeTab(tab: ReportTab): void {
    this.activeTab = tab;

    this.pageNumber = 1;

    this.loadReport();
  }

  // ========================================
  // LOAD CURRENT REPORT
  // ========================================

  loadReport(): void {
    this.errorMessage = '';

    switch (this.activeTab) {
      case 'inventory-stock':
        this.loadInventoryStock();
        break;

      case 'inventory-transactions':
        this.loadInventoryTransactions();
        break;

      case 'low-stock':
        this.loadLowStock();
        break;

      case 'purchase-orders':
        this.loadPurchaseOrders();
        break;
    }
  }

  // ========================================
  // INVENTORY STOCK
  // ========================================

  loadInventoryStock(): void {
    this.reportService
      .getInventoryStockReport({
        pageNumber: this.pageNumber,

        pageSize: this.pageSize,

        warehouseId: this.warehouseId,

        productId: this.productId,

        categoryId: this.categoryId,
      })
      .subscribe({
        next: (response) => {
          this.inventoryStockData = response?.data?.items ?? [];

          this.inventoryStockSummary = response?.summary ?? {
            totalRecords: 0,
            totalQuantity: 0,
            totalInventoryValue: 0,
            lowStockCount: 0,
            outOfStockCount: 0,
          };

          this.setPagination(response?.data);

          this.loading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.handleError(error, 'Failed to load inventory stock report.');
        },
      });
  }

  // ========================================
  // INVENTORY TRANSACTIONS
  // ========================================

  loadInventoryTransactions(): void {
    this.reportService
      .getInventoryTransactionReport({
        pageNumber: this.pageNumber,

        pageSize: this.pageSize,

        warehouseId: this.warehouseId,

        productId: this.productId,

        transactionType: this.transactionType,

        fromDate: this.fromDate || undefined,

        toDate: this.toDate || undefined,
      })
      .subscribe({
        next: (response) => {
          this.inventoryTransactionData = response?.data?.items ?? [];

          this.inventoryTransactionSummary = response?.summary ?? {
            totalTransactions: 0,
            totalTransactionQuantity: 0,
            stockInTransactions: 0,
            stockOutTransactions: 0,
          };

          this.setPagination(response?.data);

          this.loading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.handleError(error, 'Failed to load inventory transactions.');
        },
      });
  }

  // ========================================
  // LOW STOCK
  // ========================================

  loadLowStock(): void {
    this.reportService
      .getLowStockReport({
        pageNumber: this.pageNumber,

        pageSize: this.pageSize,

        warehouseId: this.warehouseId,

        productId: this.productId,

        categoryId: this.categoryId,
      })
      .subscribe({
        next: (response) => {
          this.lowStockData = response?.data?.items ?? [];

          this.lowStockSummary = response?.summary ?? {
            totalLowStockProducts: 0,
            totalShortageQuantity: 0,
            outOfStockProducts: 0,
          };

          this.setPagination(response?.data);

          this.loading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.handleError(error, 'Failed to load low stock report.');
        },
      });
  }

  // ========================================
  // PURCHASE ORDERS
  // ========================================

  loadPurchaseOrders(): void {
    this.reportService
      .getPurchaseOrderReport({
        pageNumber: this.pageNumber,

        pageSize: this.pageSize,

        supplierId: this.supplierId,

        warehouseId: this.warehouseId,

        status: this.purchaseOrderStatus,

        fromDate: this.fromDate || undefined,

        toDate: this.toDate || undefined,
      })
      .subscribe({
        next: (response) => {
          this.purchaseOrderData = response?.data?.items ?? [];

          this.purchaseOrderSummary = response?.summary ?? {
            totalPurchaseOrders: 0,
            totalPurchaseAmount: 0,
            draftCount: 0,
            sentCount: 0,
            confirmedCount: 0,
            partiallyReceivedCount: 0,
            completedCount: 0,
            cancelledCount: 0,
          };

          this.setPagination(response?.data);

          this.loading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.handleError(error, 'Failed to load purchase order report.');
        },
      });
  }

  // ========================================
  // FILTERS
  // ========================================

  applyFilters(): void {
    this.pageNumber = 1;

    this.loadReport();
  }

  resetFilters(): void {
    this.warehouseId = undefined;

    this.productId = undefined;

    this.categoryId = undefined;

    this.supplierId = undefined;

    this.transactionType = undefined;

    this.purchaseOrderStatus = undefined;

    this.fromDate = '';

    this.toDate = '';

    this.pageNumber = 1;

    this.loadReport();
  }

  // ========================================
  // PAGINATION
  // ========================================

  setPagination(data: any): void {
    this.totalCount = data?.totalCount ?? 0;

    this.totalPages = data?.totalPages ?? 0;
  }

  nextPage(): void {
    if (this.pageNumber < this.totalPages) {
      this.pageNumber++;

      this.loadReport();
    }
  }

  previousPage(): void {
    if (this.pageNumber > 1) {
      this.pageNumber--;

      this.loadReport();
    }
  }

  // ========================================
  // CHART HELPERS
  // ========================================

  getStockInPercentage(): number {
    const total =
      this.inventoryTransactionSummary.stockInTransactions +
      this.inventoryTransactionSummary.stockOutTransactions;

    if (!total) {
      return 0;
    }

    return (this.inventoryTransactionSummary.stockInTransactions / total) * 100;
  }

  getStockOutPercentage(): number {
    const total =
      this.inventoryTransactionSummary.stockInTransactions +
      this.inventoryTransactionSummary.stockOutTransactions;

    if (!total) {
      return 0;
    }

    return (this.inventoryTransactionSummary.stockOutTransactions / total) * 100;
  }

  getPurchaseStatusTotal(): number {
    return (
      this.purchaseOrderSummary.draftCount +
      this.purchaseOrderSummary.sentCount +
      this.purchaseOrderSummary.confirmedCount +
      this.purchaseOrderSummary.partiallyReceivedCount +
      this.purchaseOrderSummary.completedCount +
      this.purchaseOrderSummary.cancelledCount
    );
  }

  getPurchaseStatusPercentage(value: number): number {
    const total = this.getPurchaseStatusTotal();

    if (!total) {
      return 0;
    }

    return (value / total) * 100;
  }

  getMaxWarehouseStock(): number {
    if (!this.inventoryStockData.length) {
      return 1;
    }

    return Math.max(...this.inventoryStockData.map((x) => x.currentQuantity), 1);
  }

  getStockBarWidth(quantity: number): number {
    return (quantity / this.getMaxWarehouseStock()) * 100;
  }

  getMaxShortage(): number {
    if (!this.lowStockData.length) {
      return 1;
    }

    return Math.max(...this.lowStockData.map((x) => x.shortageQuantity), 1);
  }

  getShortageBarWidth(shortage: number): number {
    return (shortage / this.getMaxShortage()) * 100;
  }

  // ========================================
  // FORMAT
  // ========================================

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      maximumFractionDigits: 2,
    }).format(value ?? 0);
  }

  formatNumber(value: number): string {
    return new Intl.NumberFormat('en-IN').format(value ?? 0);
  }

  // ========================================
  // ERROR
  // ========================================

  private handleError(error: any, fallback: string): void {
    console.error('Report API error:', error);

    this.errorMessage = error?.error?.message ?? fallback;

    this.loading = false;

    this.cdr.detectChanges();
  }
}
