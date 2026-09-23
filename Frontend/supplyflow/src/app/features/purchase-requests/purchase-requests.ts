import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { PurchaseRequestService } from '../../core/services/purchase-request.service';

import { ProductService } from '../../core/services/product.service';

import {
  PurchaseRequest,
  CreatePurchaseRequestRequest,
} from '../../core/models/purchase-request.model';

import { Product } from '../../core/models/product.model';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-purchase-requests',

  standalone: true,

  imports: [CommonModule, ReactiveFormsModule, FormsModule],

  templateUrl: './purchase-requests.html',

  styleUrl: './purchase-requests.css',
})
export class PurchaseRequests implements OnInit {
  purchaseRequests: PurchaseRequest[] = [];

  filteredRequests: PurchaseRequest[] = [];

  products: Product[] = [];

  isLoading = false;

  isSubmitting = false;

  errorMessage = '';

  successMessage = '';

  searchTerm = '';

  selectedStatus = 'All';

  showCreateModal = false;

  showViewModal = false;

  showRejectModal = false;

  selectedRequest: PurchaseRequest | null = null;

  purchaseRequestForm: FormGroup;

  rejectForm: FormGroup;

  statuses = ['All', 'Draft', 'Submitted', 'Approved', 'Rejected', 'Cancelled'];

  constructor(
    private readonly fb: FormBuilder,

    private readonly purchaseRequestService: PurchaseRequestService,

    private readonly productService: ProductService,

    private readonly authService: AuthService,

    private readonly cdr: ChangeDetectorRef,
  ) {
    this.purchaseRequestForm = this.fb.group({
      remarks: [''],

      items: this.fb.array([]),
    });

    this.rejectForm = this.fb.group({
      rejectionReason: ['', [Validators.required]],
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

  canCreateRequest(): boolean {
    return this.isEmployee() || this.isAdmin();
  }

  canSubmitRequest(): boolean {
    return this.isEmployee();
  }

  canApproveRequest(): boolean {
    return this.isAdmin() || this.isPurchaseManager();
  }

  canRejectRequest(): boolean {
    return this.isAdmin() || this.isPurchaseManager();
  }

  ngOnInit(): void {
    this.loadPurchaseRequests();

    this.loadProducts();
  }

  get items(): FormArray {
    return this.purchaseRequestForm.get('items') as FormArray;
  }

  loadPurchaseRequests(): void {
    this.isLoading = true;

    this.errorMessage = '';

    this.cdr.detectChanges();

    this.purchaseRequestService.getAll().subscribe({
      next: (requests) => {
        this.purchaseRequests = requests ?? [];

        this.applyFilters();

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to load purchase requests.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  loadProducts(): void {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products = (products ?? []).filter((product) => product.isActive);

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to load products.';

        this.cdr.detectChanges();
      },
    });
  }

  applyFilters(): void {
    const search = this.searchTerm.trim().toLowerCase();

    this.filteredRequests = this.purchaseRequests.filter((request) => {
      const matchesSearch =
        !search ||
        request.requestNumber.toLowerCase().includes(search) ||
        request.requestedByUserName.toLowerCase().includes(search);

      const matchesStatus = this.selectedStatus === 'All' || request.status === this.selectedStatus;

      return matchesSearch && matchesStatus;
    });

    this.cdr.detectChanges();
  }

  openCreateModal(): void {
    this.errorMessage = '';

    this.successMessage = '';

    this.purchaseRequestForm.reset();

    this.items.clear();

    this.addItem();

    this.showCreateModal = true;

    this.cdr.detectChanges();
  }

  closeCreateModal(): void {
    this.showCreateModal = false;

    this.isSubmitting = false;

    this.cdr.detectChanges();
  }

  addItem(): void {
    this.items.push(
      this.fb.group({
        productId: [null, [Validators.required]],

        requestedQuantity: [1, [Validators.required, Validators.min(0.01)]],

        remarks: [''],
      }),
    );

    this.cdr.detectChanges();
  }

  removeItem(index: number): void {
    if (this.items.length <= 1) {
      return;
    }

    this.items.removeAt(index);

    this.cdr.detectChanges();
  }

  createPurchaseRequest(): void {
    if (this.purchaseRequestForm.invalid) {
      this.purchaseRequestForm.markAllAsTouched();

      this.cdr.detectChanges();

      return;
    }

    this.isSubmitting = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    const formValue = this.purchaseRequestForm.value;

    const request: CreatePurchaseRequestRequest = {
      remarks: formValue.remarks || undefined,

      items: formValue.items.map(
        (item: { productId: number; requestedQuantity: number; remarks?: string }) => ({
          productId: Number(item.productId),

          requestedQuantity: Number(item.requestedQuantity),

          remarks: item.remarks || undefined,
        }),
      ),
    };

    this.purchaseRequestService.create(request).subscribe({
      next: () => {
        this.successMessage = 'Purchase request created successfully.';

        this.isSubmitting = false;

        this.showCreateModal = false;

        this.loadPurchaseRequests();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to create purchase request.';

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  openViewModal(request: PurchaseRequest): void {
    this.selectedRequest = request;

    this.showViewModal = true;

    this.cdr.detectChanges();
  }

  closeViewModal(): void {
    this.showViewModal = false;

    this.selectedRequest = null;

    this.cdr.detectChanges();
  }

  submitRequest(request: PurchaseRequest): void {
    this.isLoading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.purchaseRequestService.submit(request.id).subscribe({
      next: () => {
        this.successMessage = 'Purchase request submitted successfully.';

        this.loadPurchaseRequests();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to submit purchase request.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  approveRequest(request: PurchaseRequest): void {
    this.isLoading = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.purchaseRequestService.approve(request.id).subscribe({
      next: () => {
        this.successMessage = 'Purchase request approved successfully.';

        this.loadPurchaseRequests();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to approve purchase request.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  openRejectModal(request: PurchaseRequest): void {
    this.selectedRequest = request;

    this.rejectForm.reset();

    this.showRejectModal = true;

    this.cdr.detectChanges();
  }

  closeRejectModal(): void {
    this.showRejectModal = false;

    this.selectedRequest = null;

    this.rejectForm.reset();

    this.cdr.detectChanges();
  }

  rejectRequest(): void {
    if (this.rejectForm.invalid || !this.selectedRequest) {
      this.rejectForm.markAllAsTouched();

      this.cdr.detectChanges();

      return;
    }

    this.isSubmitting = true;

    this.errorMessage = '';

    this.successMessage = '';

    this.cdr.detectChanges();

    this.purchaseRequestService
      .reject(this.selectedRequest.id, {
        rejectionReason: this.rejectForm.value.rejectionReason,
      })
      .subscribe({
        next: () => {
          this.successMessage = 'Purchase request rejected successfully.';

          this.isSubmitting = false;

          this.showRejectModal = false;

          this.selectedRequest = null;

          this.rejectForm.reset();

          this.loadPurchaseRequests();

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.errorMessage = error?.error?.message || 'Failed to reject purchase request.';

          this.isSubmitting = false;

          this.cdr.detectChanges();
        },
      });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'draft':
        return 'bg-slate-100 text-slate-700';

      case 'submitted':
        return 'bg-blue-100 text-blue-700';

      case 'approved':
        return 'bg-green-100 text-green-700';

      case 'rejected':
        return 'bg-red-100 text-red-700';

      case 'cancelled':
        return 'bg-orange-100 text-orange-700';

      default:
        return 'bg-gray-100 text-gray-700';
    }
  }

  trackByRequestId(index: number, request: PurchaseRequest): number {
    return request.id;
  }

  trackByItemIndex(index: number): number {
    return index;
  }
}
