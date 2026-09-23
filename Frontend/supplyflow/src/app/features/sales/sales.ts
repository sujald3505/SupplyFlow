import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { forkJoin } from 'rxjs';

import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { SaleService } from '../../core/services/sale.service';
import { ProductService } from '../../core/services/product.service';
import { WarehouseService } from '../../core/services/warehouse.service';
import { CustomerService } from '../../core/services/customer.service';

import { Sale,CreateSaleRequest, CreateSaleItemRequest } from '../../core/models/sale.model';

import { Customer,CreateCustomerRequest } from '../../core/models/customer.model';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,FormsModule,RouterLink],
  templateUrl: './sales.html',
  styleUrl: './sales.css',
})
export class Sales implements OnInit {
  sales: Sale[] = [];

  products: any[] = [];

  warehouses: any[] = [];

  customers: Customer[] = [];

  isLoading = true;

  isSubmitting = false;

  isCustomerSubmitting = false;

  showForm = false;

  showCustomerForm = false;

  errorMessage = '';

  customerErrorMessage = '';

  selectedSale: Sale | null = null;

  saleForm;

  customerForm;

  items: CreateSaleItemRequest[] = [];

  constructor(
    private readonly fb: FormBuilder,

    private readonly saleService: SaleService,

    private readonly productService: ProductService,

    private readonly warehouseService: WarehouseService,

    private readonly customerService: CustomerService,

    private readonly cdr: ChangeDetectorRef,
  ) {
    this.saleForm = this.fb.nonNullable.group({
      warehouseId: [0, [Validators.required, Validators.min(1)]],

      customerId: [0, [Validators.required, Validators.min(1)]],

      discount: [0, [Validators.required, Validators.min(0)]],

      tax: [0, [Validators.required, Validators.min(0)]],

      remarks: ['', Validators.maxLength(500)],
    });

    this.customerForm = this.fb.nonNullable.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],

      phone: ['', Validators.maxLength(20)],

      email: ['', Validators.email],

      address: ['', Validators.maxLength(500)],

      gstin: ['', Validators.maxLength(50)],
    });
  }

  ngOnInit(): void {
    this.loadInitialData();
  }

  // =====================================================
  // LOAD INITIAL DATA
  // =====================================================

  loadInitialData(): void {
    this.isLoading = true;

    this.errorMessage = '';

    forkJoin({
      sales: this.saleService.getAll(),

      products: this.productService.getAll(),

      warehouses: this.warehouseService.getAll(),

      customers: this.customerService.getAll(),
    }).subscribe({
      next: (response) => {
        this.sales = response.sales;

        this.products = response.products;

        this.warehouses = response.warehouses;

        this.customers = response.customers;

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message ?? 'Failed to load sales data.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  // =====================================================
  // OPEN SALE FORM
  // =====================================================

  openAddForm(): void {
    this.selectedSale = null;

    this.showForm = true;

    this.errorMessage = '';

    this.items = [];

    this.saleForm.reset({
      warehouseId: 0,
      customerId: 0,
      discount: 0,
      tax: 0,
      remarks: '',
    });

    this.cdr.detectChanges();
  }

  // =====================================================
  // CLOSE SALE FORM
  // =====================================================

  closeForm(): void {
    this.showForm = false;

    this.selectedSale = null;

    this.items = [];

    this.saleForm.reset({
      warehouseId: 0,
      customerId: 0,
      discount: 0,
      tax: 0,
      remarks: '',
    });

    this.cdr.detectChanges();
  }

  // =====================================================
  // CUSTOMER
  // =====================================================

  openCustomerForm(): void {
    this.customerErrorMessage = '';

    this.customerForm.reset({
      name: '',
      phone: '',
      email: '',
      address: '',
      gstin: '',
    });

    this.showCustomerForm = true;

    this.cdr.detectChanges();
  }

  closeCustomerForm(): void {
    if (this.isCustomerSubmitting) {
      return;
    }

    this.showCustomerForm = false;

    this.customerErrorMessage = '';

    this.customerForm.reset({
      name: '',
      phone: '',
      email: '',
      address: '',
      gstin: '',
    });

    this.cdr.detectChanges();
  }

  createCustomer(): void {
    if (this.customerForm.invalid) {
      this.customerForm.markAllAsTouched();

      return;
    }

    this.isCustomerSubmitting = true;

    this.customerErrorMessage = '';

    const value = this.customerForm.getRawValue();

    const request: CreateCustomerRequest = {
      name: value.name.trim(),

      phone: value.phone?.trim() || null,

      email: value.email?.trim() || null,

      address: value.address?.trim() || null,

      gstin: value.gstin?.trim() || null,
    };

    this.customerService.create(request).subscribe({
      next: (customer) => {
        /*
         * Add newly created customer
         * into dropdown.
         */

        this.customers = [customer, ...this.customers];

        /*
         * Automatically select
         * newly created customer.
         */

        this.saleForm.patchValue({
          customerId: customer.id,
        });

        this.isCustomerSubmitting = false;

        this.showCustomerForm = false;

        this.customerForm.reset({
          name: '',
          phone: '',
          email: '',
          address: '',
          gstin: '',
        });

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.customerErrorMessage = error?.error?.message ?? 'Failed to create customer.';

        this.isCustomerSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  // =====================================================
  // PRODUCT
  // =====================================================

  addProduct(productId: number): void {
    if (!productId || productId <= 0) {
      return;
    }

    const product = this.products.find((x) => x.id === Number(productId));

    if (!product) {
      return;
    }

    const existingItem = this.items.find((x) => x.productId === Number(productId));

    if (existingItem) {
      existingItem.quantity += 1;
    } else {
      this.items.push({
        productId: product.id,

        quantity: 1,

        unitPrice: Number(product.salePrice ?? product.sellingPrice ?? product.price ?? 0),

        discount: 0,

        tax: 0,
      });
    }

    this.cdr.detectChanges();
  }

  removeItem(index: number): void {
    if (index < 0 || index >= this.items.length) {
      return;
    }

    this.items.splice(index, 1);

    this.cdr.detectChanges();
  }

  getProduct(productId: number): any {
    return this.products.find((x) => x.id === Number(productId));
  }

  getProductName(productId: number): string {
    const product = this.getProduct(productId);

    return product?.name ?? product?.productName ?? 'Unknown Product';
  }

  getProductSku(productId: number): string {
    const product = this.getProduct(productId);

    return product?.sku ?? '';
  }

  // =====================================================
  // ITEM TOTAL
  // =====================================================

  getItemTotal(item: CreateSaleItemRequest): number {
    const quantity = Number(item.quantity) || 0;

    const unitPrice = Number(item.unitPrice) || 0;

    const discount = Number(item.discount) || 0;

    const tax = Number(item.tax) || 0;

    const gross = quantity * unitPrice;

    const afterDiscount = Math.max(gross - discount, 0);

    return afterDiscount + tax;
  }

  // =====================================================
  // SUB TOTAL
  // =====================================================

  getSubTotal(): number {
    return this.items.reduce(
      (total, item) => total + (Number(item.quantity) || 0) * (Number(item.unitPrice) || 0),
      0,
    );
  }

  // =====================================================
  // ITEM DISCOUNT
  // =====================================================

  getItemDiscount(): number {
    return this.items.reduce((total, item) => total + (Number(item.discount) || 0), 0);
  }

  // =====================================================
  // ITEM TAX
  // =====================================================

  getItemTax(): number {
    return this.items.reduce((total, item) => total + (Number(item.tax) || 0), 0);
  }

  // =====================================================
  // GRAND TOTAL
  // =====================================================

  getGrandTotal(): number {
    const subTotal = this.getSubTotal();

    const itemDiscount = this.getItemDiscount();

    const itemTax = this.getItemTax();

    const saleDiscount = Number(this.saleForm.controls.discount.value) || 0;

    const saleTax = Number(this.saleForm.controls.tax.value) || 0;

    return Math.max(subTotal - itemDiscount - saleDiscount + itemTax + saleTax, 0);
  }

  // =====================================================
  // CREATE SALE
  // =====================================================

  createSale(): void {
    if (this.saleForm.invalid) {
      this.saleForm.markAllAsTouched();

      return;
    }

    if (this.items.length === 0) {
      this.errorMessage = 'Please add at least one product.';

      return;
    }

    this.isSubmitting = true;

    this.errorMessage = '';

    const value = this.saleForm.getRawValue();

    const request: CreateSaleRequest = {
      warehouseId: value.warehouseId,

      customerId: value.customerId,

      discount: value.discount,

      tax: value.tax,

      remarks: value.remarks || undefined,

      items: this.items.map((item) => ({
        productId: Number(item.productId),

        quantity: Number(item.quantity),

        unitPrice: Number(item.unitPrice),

        discount: Number(item.discount),

        tax: Number(item.tax),
      })),
    };

    this.saleService.create(request).subscribe({
      next: (sale) => {
        this.sales = [sale, ...this.sales];

        this.isSubmitting = false;

        this.closeForm();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message ?? 'Failed to create sale.';

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  // =====================================================
  // VIEW SALE
  // =====================================================

  viewSale(sale: Sale): void {
    this.selectedSale = sale;
  }

  // =====================================================
  // CLOSE DETAILS
  // =====================================================

  closeDetails(): void {
    this.selectedSale = null;
  }

  // =====================================================
  // TOTAL ITEMS
  // =====================================================

  getTotalItems(sale: Sale): number {
    return sale.items.reduce((total, item) => total + item.quantity, 0);
  }

  // =====================================================
  // CUSTOMER NAME
  // =====================================================

  getCustomerName(customerId: number): string {
    const customer = this.customers.find((x) => x.id === customerId);

    return customer?.name ?? 'Unknown Customer';
  }
}
