import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { CommonModule, DatePipe } from '@angular/common';

import {
  FormArray,FormBuilder,FormGroup,FormsModule,ReactiveFormsModule,Validators,} from '@angular/forms';

import { PurchaseOrder, CreatePurchaseOrder } from '../../core/models/purchase-order.model';

import { Product } from '../../core/models/product.model';
import { Supplier } from '../../core/models/supplier.model';
import { Warehouse } from '../../core/models/warehouse.model';

import { PurchaseOrderService } from '../../core/services/purchase-order.service';
import { ProductService } from '../../core/services/product.service';
import { SupplierService } from '../../core/services/supplier.service';
import { WarehouseService } from '../../core/services/warehouse.service';
import { PurchaseRequest } from '../../core/models/purchase-request.model';

import { PurchaseRequestService } from '../../core/services/purchase-request.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-purchase-orders',

  standalone: true,

  imports: [CommonModule, ReactiveFormsModule, DatePipe, FormsModule],

  templateUrl: './purchase-orders.html',

  styleUrl: './purchase-orders.css',
})
export class PurchaseOrders implements OnInit {
  purchaseOrders: PurchaseOrder[] = [];

  filteredPurchaseOrders: PurchaseOrder[] = [];

  products: Product[] = [];

  suppliers: Supplier[] = [];

  warehouses: Warehouse[] = [];

  purchaseRequests: PurchaseRequest[] = [];

  approvedPurchaseRequests: PurchaseRequest[] = [];

  isLoading = false;

  isSubmitting = false;

  showCreateModal = false;

  showViewModal = false;

  selectedPurchaseOrder: PurchaseOrder | null = null;

  searchTerm = '';

  selectedStatus = 'All';

  statuses = ['All', 'Draft', 'Sent', 'Confirmed', 'Cancelled'];

  successMessage = '';

  errorMessage = '';

  purchaseOrderForm: FormGroup;

  constructor(
    private readonly fb: FormBuilder,

    private readonly purchaseOrderService: PurchaseOrderService,

    private readonly productService: ProductService,

    private readonly supplierService: SupplierService,

    private readonly warehouseService: WarehouseService,

    private readonly purchaseRequestService: PurchaseRequestService,

    private readonly authService: AuthService,

    private readonly cdr: ChangeDetectorRef,
  ) {
    this.purchaseOrderForm = this.fb.group({
      supplierId: [null, Validators.required],

      warehouseId: [null, Validators.required],

      purchaseRequestId: [null],

      expectedDeliveryDate: [''],

      remarks: [''],

      items: this.fb.array([this.createItem()]),
    });
  }

  ngOnInit(): void {
    this.loadPurchaseOrders();

    this.loadProducts();

    this.loadSuppliers();

    this.loadWarehouses();

    this.loadPurchaseRequests();
  }

  get items(): FormArray {
    return this.purchaseOrderForm.get('items') as FormArray;
  }

  private createItem(): FormGroup {
    return this.fb.group({
      productId: [null, Validators.required],

      orderedQuantity: [1, [Validators.required, Validators.min(0.01)]],

      unitPrice: [0, [Validators.required, Validators.min(0)]],
    });
  }

  loadPurchaseOrders(): void {
    this.isLoading = true;

    this.purchaseOrderService.getAll().subscribe({
      next: (data) => {
        this.purchaseOrders = data;

        this.applyFilters();

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to load purchase orders.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }
  private loadPurchaseRequests(): void {
    this.purchaseRequestService.getAll().subscribe({
      next: (requests) => {
        this.purchaseRequests = requests;

        this.approvedPurchaseRequests = requests.filter((request) => request.status === 'Approved');
      },
      error: () => {
        this.approvedPurchaseRequests = [];
      },
    });
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (data) => {
        this.products = data.filter((product) => product.isActive);

        this.cdr.detectChanges();
      },

      error: () => {
        this.errorMessage = 'Failed to load products.';

        this.cdr.detectChanges();
      },
    });
  }

  loadSuppliers(): void {
    this.supplierService.getAll().subscribe({
      next: (data) => {
        this.suppliers = data.filter((supplier) => supplier.isActive);

        this.cdr.detectChanges();
      },

      error: () => {
        this.errorMessage = 'Failed to load suppliers.';

        this.cdr.detectChanges();
      },
    });
  }
  onPurchaseRequestChange(): void {
    const purchaseRequestId = this.purchaseOrderForm.get('purchaseRequestId')?.value;

    if (!purchaseRequestId) {
      return;
    }

    const request = this.approvedPurchaseRequests.find((pr) => pr.id === Number(purchaseRequestId));

    if (!request) {
      return;
    }

    const itemsArray = this.purchaseOrderForm.get('items') as FormArray;

    while (itemsArray.length) {
      itemsArray.removeAt(0);
    }

    request.items.forEach((item) => {
      const product = this.products.find((p) => p.id === item.productId);

      itemsArray.push(
        this.fb.group({
          productId: [item.productId, Validators.required],
          orderedQuantity: [item.requestedQuantity, [Validators.required, Validators.min(0.01)]],
          unitPrice: [product?.costPrice ?? 0, [Validators.required, Validators.min(0)]],
        }),
      );
    });
  }

  loadWarehouses(): void {
    this.warehouseService.getAll().subscribe({
      next: (data) => {
        this.warehouses = data.filter((warehouse) => warehouse.isActive);

        this.cdr.detectChanges();
      },

      error: () => {
        this.errorMessage = 'Failed to load warehouses.';

        this.cdr.detectChanges();
      },
    });
  }

  applyFilters(): void {
    const search = this.searchTerm.trim().toLowerCase();

    this.filteredPurchaseOrders = this.purchaseOrders.filter((purchaseOrder) => {
      const matchesSearch =
        !search ||
        purchaseOrder.purchaseOrderNumber.toLowerCase().includes(search) ||
        purchaseOrder.supplierName.toLowerCase().includes(search) ||
        purchaseOrder.warehouseName.toLowerCase().includes(search);

      const matchesStatus =
        this.selectedStatus === 'All' || purchaseOrder.status === this.selectedStatus;

      return matchesSearch && matchesStatus;
    });

    this.cdr.detectChanges();
  }

  openCreateModal(): void {
    this.successMessage = '';

    this.errorMessage = '';

    this.purchaseOrderForm.reset();

    this.items.clear();

    this.items.push(this.createItem());

    this.showCreateModal = true;

    this.cdr.detectChanges();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;

    this.cdr.detectChanges();
  }

  addItem(): void {
    this.items.push(this.createItem());

    this.cdr.detectChanges();
  }

  removeItem(index: number): void {
    if (this.items.length > 1) {
      this.items.removeAt(index);

      this.cdr.detectChanges();
    }
  }

  onProductChange(index: number): void {
    const item = this.items.at(index);

    const productId = Number(item.get('productId')?.value);

    const product = this.products.find((item) => item.id === productId);

    if (product) {
      item.patchValue({
        unitPrice: product.costPrice,
      });

      this.cdr.detectChanges();
    }
  }

  createPurchaseOrder(): void {
    if (this.purchaseOrderForm.invalid) {
      this.purchaseOrderForm.markAllAsTouched();

      return;
    }

    this.isSubmitting = true;

    const formValue = this.purchaseOrderForm.value;

    const request: CreatePurchaseOrder = {
      supplierId: Number(formValue.supplierId),

      warehouseId: Number(formValue.warehouseId),

      purchaseRequestId: formValue.purchaseRequestId
        ? Number(formValue.purchaseRequestId)
        : undefined,

      expectedDeliveryDate: formValue.expectedDeliveryDate || undefined,

      remarks: formValue.remarks || undefined,

      items: formValue.items.map(
        (item: { productId: number; orderedQuantity: number; unitPrice: number }) => ({
          productId: Number(item.productId),

          orderedQuantity: Number(item.orderedQuantity),

          unitPrice: Number(item.unitPrice),
        }),
      ),
    };

    this.purchaseOrderService.create(request).subscribe({
      next: () => {
        this.successMessage = 'Purchase order created successfully.';

        this.isSubmitting = false;

        this.closeCreateModal();

        this.loadPurchaseOrders();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to create purchase order.';

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  openViewModal(purchaseOrder: PurchaseOrder): void {
    this.selectedPurchaseOrder = purchaseOrder;

    this.showViewModal = true;

    this.cdr.detectChanges();
  }

  closeViewModal(): void {
    this.showViewModal = false;

    this.selectedPurchaseOrder = null;

    this.cdr.detectChanges();
  }

  sendPurchaseOrder(purchaseOrder: PurchaseOrder): void {
    this.isSubmitting = true;

    this.purchaseOrderService.send(purchaseOrder.id).subscribe({
      next: () => {
        this.successMessage = 'Purchase order sent successfully.';

        this.isSubmitting = false;

        this.loadPurchaseOrders();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to send purchase order.';

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  confirmPurchaseOrder(purchaseOrder: PurchaseOrder): void {
    this.isSubmitting = true;

    this.purchaseOrderService.confirm(purchaseOrder.id).subscribe({
      next: () => {
        this.successMessage = 'Purchase order confirmed successfully.';

        this.isSubmitting = false;

        this.loadPurchaseOrders();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to confirm purchase order.';

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  cancelPurchaseOrder(purchaseOrder: PurchaseOrder): void {
    this.isSubmitting = true;

    this.purchaseOrderService.cancel(purchaseOrder.id).subscribe({
      next: () => {
        this.successMessage = 'Purchase order cancelled successfully.';

        this.isSubmitting = false;

        this.loadPurchaseOrders();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to cancel purchase order.';

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  // ==========================================
  // ROLE CHECKS
  // ==========================================

  private get currentUserRole(): string {
    return this.authService.getCurrentUser()?.role?.trim().toLowerCase() || '';
  }

  isAdmin(): boolean {
    return this.currentUserRole === 'admin';
  }

  isPurchaseManager(): boolean {
    return this.currentUserRole === 'purchasemanager';
  }

  isWarehouseManager(): boolean {
    return this.currentUserRole === 'warehousemanager';
  }

  isEmployee(): boolean {
    return this.currentUserRole === 'employee';
  }

  canManagePurchaseOrder(): boolean {
    return this.isAdmin() || this.isPurchaseManager();
  }

  calculateItemTotal(index: number): number {
    const item = this.items.at(index);

    const quantity = Number(item.get('orderedQuantity')?.value) || 0;

    const price = Number(item.get('unitPrice')?.value) || 0;

    return quantity * price;
  }

  calculateTotalAmount(): number {
    return this.items.controls.reduce(
      (total, _, index) => total + this.calculateItemTotal(index),
      0,
    );
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'draft':
        return 'bg-slate-100 text-slate-700';

      case 'sent':
        return 'bg-blue-100 text-blue-700';

      case 'confirmed':
        return 'bg-green-100 text-green-700';

      case 'cancelled':
        return 'bg-red-100 text-red-700';

      default:
        return 'bg-slate-100 text-slate-700';
    }
  }

  canSend(purchaseOrder: PurchaseOrder): boolean {
    return this.canManagePurchaseOrder() && purchaseOrder.status.toLowerCase() === 'draft';
  }

  canConfirm(purchaseOrder: PurchaseOrder): boolean {
    return this.canManagePurchaseOrder() && purchaseOrder.status.toLowerCase() === 'sent';
  }

  canCancel(purchaseOrder: PurchaseOrder): boolean {
    const status = purchaseOrder.status.toLowerCase();

    return this.canManagePurchaseOrder() && (status === 'draft' || status === 'sent');
  }

  trackByPurchaseOrderId(index: number, purchaseOrder: PurchaseOrder): number {
    return purchaseOrder.id;
  }

  trackByItemIndex(index: number): number {
    return index;
  }
}
