import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SaleService } from '../../core/services/sale.service';
import { SalesReturnService } from '../../core/services/sales-return.service';

import { Sale, SaleItem } from '../../core/models/sale.model';

import {
  CreateSalesReturnRequest,
  CreateSalesReturnItemRequest,
  SalesReturn,
} from '../../core/models/sales-return.model';

@Component({
  selector: 'app-sales-return',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './sales-return.html',
  styleUrl: './sales-return.css',
})
export class SalesReturnComponent implements OnInit {
  // =========================================================
  // DATA
  // =========================================================

  sales: Sale[] = [];

  selectedSale: Sale | null = null;

  selectedItems: ReturnItem[] = [];

  createdReturn: SalesReturn | null = null;

  // =========================================================
  // FORM
  // =========================================================

  saleId: number | null = null;

  warehouseId = 0;
  customerId = 0;

  discount = 0;
  tax = 0;

  remarks = '';

  // =========================================================
  // UI STATE
  // =========================================================

  isLoading = false;
  isSaving = false;

  errorMessage = '';
  successMessage = '';

  constructor(
    private readonly saleService: SaleService,
    private readonly salesReturnService: SalesReturnService,
    private readonly cdr: ChangeDetectorRef,
  ) {}

  // =========================================================
  // INIT
  // =========================================================

  ngOnInit(): void {
    this.loadSales();
  }

  // =========================================================
  // LOAD SALES
  // =========================================================

  loadSales(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.saleService.getAll().subscribe({
      next: (response) => {
        this.sales = response ?? [];

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Failed to load sales:', error);

        this.errorMessage = error?.error?.message ?? 'Failed to load sales.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  // =========================================================
  // SALE SELECT
  // =========================================================

  onSaleChange(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.createdReturn = null;

    this.selectedSale = null;
    this.selectedItems = [];

    if (!this.saleId) {
      this.warehouseId = 0;
      this.customerId = 0;

      return;
    }

    const sale = this.sales.find((x) => x.id === Number(this.saleId));

    if (!sale) {
      this.errorMessage = 'Selected sale not found.';

      return;
    }

    this.selectedSale = sale;

    this.warehouseId = sale.warehouseId;

    this.customerId = sale.customerId;

    // Load all sale items as return candidates
    this.selectedItems = sale.items.map((item) => ({
      saleItemId: item.id,
      productId: item.productId,

      productName: item.productName,
      sku: item.sku,

      soldQuantity: item.quantity,

      alreadyReturnedQuantity: 0,

      remainingQuantity: item.quantity,

      quantity: 0,

      unitPrice: item.unitPrice,

      discount: 0,

      tax: 0,

      total: 0,

      selected: false,
    }));

    this.calculateTotals();

    this.cdr.detectChanges();
  }

  // =========================================================
  // SELECT / UNSELECT ITEM
  // =========================================================

  toggleItem(item: ReturnItem): void {
    item.selected = !item.selected;

    if (!item.selected) {
      item.quantity = 0;

      item.discount = 0;

      item.tax = 0;

      item.total = 0;
    } else {
      // Automatically select maximum
      // available quantity only if needed.
      item.quantity = 1;

      this.validateItemQuantity(item);
    }

    this.calculateTotals();

    this.cdr.detectChanges();
  }

  // =========================================================
  // QUANTITY CHANGE
  // =========================================================

  onQuantityChange(item: ReturnItem): void {
    this.validateItemQuantity(item);

    this.calculateItemTotal(item);

    this.calculateTotals();

    this.cdr.detectChanges();
  }

  // =========================================================
  // VALIDATE QUANTITY
  // =========================================================

  validateItemQuantity(item: ReturnItem): void {
    if (item.quantity < 0) {
      item.quantity = 0;
    }

    if (item.quantity > item.remainingQuantity) {
      item.quantity = item.remainingQuantity;

      this.errorMessage = `${item.productName}: maximum return quantity is ${item.remainingQuantity}.`;
    } else {
      this.errorMessage = '';
    }
  }

  // =========================================================
  // ITEM DISCOUNT / TAX CHANGE
  // =========================================================

  onItemValueChange(item: ReturnItem): void {
    if (item.discount < 0) {
      item.discount = 0;
    }

    if (item.tax < 0) {
      item.tax = 0;
    }

    this.calculateItemTotal(item);

    this.calculateTotals();

    this.cdr.detectChanges();
  }

  // =========================================================
  // CALCULATE ITEM TOTAL
  // =========================================================

  calculateItemTotal(item: ReturnItem): void {
    const gross = item.quantity * item.unitPrice;

    const discount = Math.min(Math.max(item.discount || 0, 0), gross);

    const taxableAmount = gross - discount;

    const tax = (taxableAmount * Math.max(item.tax || 0, 0)) / 100;

    item.total = taxableAmount + tax;
  }

  // =========================================================
  // CALCULATE TOTALS
  // =========================================================

  subTotal = 0;

  totalDiscount = 0;

  totalTax = 0;

  grandTotal = 0;

  calculateTotals(): void {
    this.subTotal = 0;

    this.totalDiscount = 0;

    this.totalTax = 0;

    for (const item of this.selectedItems) {
      if (!item.selected || item.quantity <= 0) {
        continue;
      }

      this.calculateItemTotal(item);

      this.subTotal += item.quantity * item.unitPrice;

      this.totalDiscount += item.discount || 0;

      const gross = item.quantity * item.unitPrice;

      const discount = Math.min(Math.max(item.discount || 0, 0), gross);

      const taxableAmount = gross - discount;

      this.totalTax += (taxableAmount * Math.max(item.tax || 0, 0)) / 100;
    }

    const overallDiscount = Math.max(this.discount || 0, 0);

    const overallTax = Math.max(this.tax || 0, 0);

    const afterDiscount = Math.max(this.subTotal - this.totalDiscount - overallDiscount, 0);

    const itemTax = this.totalTax;

    const overallTaxAmount = (afterDiscount * overallTax) / 100;

    this.grandTotal = Math.max(
      this.subTotal - this.totalDiscount - overallDiscount + itemTax + overallTaxAmount,
      0,
    );
  }

  // =========================================================
  // FORM LEVEL VALUE CHANGE
  // =========================================================

  onFormValueChange(): void {
    this.calculateTotals();

    this.cdr.detectChanges();
  }

  // =========================================================
  // SELECTED ITEMS COUNT
  // =========================================================

  get selectedItemCount(): number {
    return this.selectedItems.filter((item) => item.selected && item.quantity > 0).length;
  }

  // =========================================================
  // CAN SUBMIT
  // =========================================================

  get canSubmit(): boolean {
    if (!this.selectedSale) {
      return false;
    }

    return this.selectedItems.some(
      (item) => item.selected && item.quantity > 0 && item.quantity <= item.remainingQuantity,
    );
  }

  // =========================================================
  // CREATE RETURN
  // =========================================================

  createReturn(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.selectedSale) {
      this.errorMessage = 'Please select an invoice.';

      return;
    }

    const returnItems = this.selectedItems.filter((item) => item.selected && item.quantity > 0);

    if (returnItems.length === 0) {
      this.errorMessage = 'Please select at least one product to return.';

      return;
    }

    // Final quantity validation
    for (const item of returnItems) {
      if (item.quantity > item.remainingQuantity) {
        this.errorMessage = `${item.productName}: return quantity cannot exceed ${item.remainingQuantity}.`;

        return;
      }
    }

    this.calculateTotals();

    const requestItems: CreateSalesReturnItemRequest[] = returnItems.map((item) => ({
      saleItemId: item.saleItemId,

      productId: item.productId,

      quantity: item.quantity,

      unitPrice: item.unitPrice,

      discount: item.discount || 0,

      tax: item.tax || 0,
    }));

    const request: CreateSalesReturnRequest = {
      saleId: this.selectedSale.id,

      warehouseId: this.warehouseId,

      customerId: this.customerId,

      discount: this.discount || 0,

      tax: this.tax || 0,

      remarks: this.remarks?.trim() || undefined,

      items: requestItems,
    };

    this.isSaving = true;

    this.salesReturnService.create(request).subscribe({
      next: (response) => {
        this.createdReturn = response;

        this.successMessage = `Sales return ${response.returnNumber} created successfully.`;

        this.isSaving = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        console.error('Sales return failed:', error);

        this.errorMessage =
          error?.error?.message ?? error?.error?.Message ?? 'Failed to create sales return.';

        this.isSaving = false;

        this.cdr.detectChanges();
      },
    });
  }

  // =========================================================
  // RESET
  // =========================================================

  resetForm(): void {
    this.saleId = null;

    this.selectedSale = null;

    this.selectedItems = [];

    this.warehouseId = 0;

    this.customerId = 0;

    this.discount = 0;

    this.tax = 0;

    this.remarks = '';

    this.subTotal = 0;

    this.totalDiscount = 0;

    this.totalTax = 0;

    this.grandTotal = 0;

    this.errorMessage = '';

    this.successMessage = '';

    this.createdReturn = null;

    this.cdr.detectChanges();
  }
}

// =========================================================
// RETURN ITEM UI MODEL
// =========================================================

interface ReturnItem {
  saleItemId: number;

  productId: number;

  productName: string;

  sku: string;

  soldQuantity: number;

  alreadyReturnedQuantity: number;

  remainingQuantity: number;

  quantity: number;

  unitPrice: number;

  discount: number;

  tax: number;

  total: number;

  selected: boolean;
}
