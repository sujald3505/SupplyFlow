import {
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';

import {
  CommonModule,
} from '@angular/common';

import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  Validators,
} from '@angular/forms';

import {
  forkJoin,
} from 'rxjs';

import {
  InventoryTransactionService,
} from '../../core/services/inventory-transaction.service';

import {
  InventoryTransaction,
  InventoryTransactionType,
  CreateInventoryTransactionRequest,
} from '../../core/models/inventory-transaction.model';

import {
  WarehouseService,
} from '../../core/services/warehouse.service';

import {
  ProductService,
} from '../../core/services/product.service';

import {
  Warehouse,
} from '../../core/models/warehouse.model';

import {
  Product,
} from '../../core/models/product.model';


@Component({
  selector: 'app-inventory-transactions',

  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
  ],

  templateUrl:
    './inventory-transactions.html',

  styleUrl:
    './inventory-transactions.css',
})
export class InventoryTransactions
  implements OnInit {


  transactions:
    InventoryTransaction[] = [];

  filteredTransactions:
    InventoryTransaction[] = [];


  warehouses:
    Warehouse[] = [];

  products:
    Product[] = [];


  searchTerm = '';

  selectedWarehouseId = 0;

  selectedProductId = 0;

  selectedTransactionType = '';


  showForm = false;

  isLoading = true;

  isSubmitting = false;

  errorMessage = '';


  transactionTypes = [
    {
      value:
        InventoryTransactionType.StockIn,

      label:
        'Stock In',
    },
    {
      value:
        InventoryTransactionType.StockOut,

      label:
        'Stock Out',
    },
    {
      value:
        InventoryTransactionType.Adjustment,

      label:
        'Adjustment',
    },
  ];


  transactionForm;


  constructor(

    private readonly fb:
      FormBuilder,

    private readonly transactionService:
      InventoryTransactionService,

    private readonly warehouseService:
      WarehouseService,

    private readonly productService:
      ProductService,

    private readonly cdr:
      ChangeDetectorRef

  ) {

    this.transactionForm =
      this.fb.nonNullable.group({

        warehouseId: [
          0,
          Validators.min(1),
        ],

        productId: [
          0,
          Validators.min(1),
        ],

        transactionType: [
          InventoryTransactionType.StockIn,
          Validators.required,
        ],

        quantity: [
          0,
          [
            Validators.required,
            Validators.min(0.01),
          ],
        ],

        referenceNumber: [
          '',
        ],

        remarks: [
          '',
        ],

      });

  }


  ngOnInit(): void {

    this.loadInitialData();

  }


  // =========================
  // LOAD INITIAL DATA
  // =========================

  loadInitialData(): void {

    this.isLoading = true;

    this.errorMessage = '';


    forkJoin({

      transactions:
        this.transactionService.getAll(),

      warehouses:
        this.warehouseService.getAll(),

      products:
        this.productService.getAll(),

    })
      .subscribe({

        next: (response) => {

          this.transactions =
            response.transactions;

          this.filteredTransactions =
            response.transactions;

          this.warehouses =
            response.warehouses;

          this.products =
            response.products;

          this.isLoading =
            false;

          this.cdr.detectChanges();

        },


        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to load inventory transactions.';

          this.isLoading =
            false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // OPEN FORM
  // =========================

  openAddForm(): void {

    this.showForm = true;

    this.errorMessage = '';


    this.transactionForm.reset({

      warehouseId: 0,

      productId: 0,

      transactionType:
        InventoryTransactionType.StockIn,

      quantity: 0,

      referenceNumber: '',

      remarks: '',

    });

    this.cdr.detectChanges();

  }


  // =========================
  // CLOSE FORM
  // =========================

  closeForm(): void {

    this.showForm = false;

    this.transactionForm.reset();

    this.cdr.detectChanges();

  }


  // =========================
  // SUBMIT
  // =========================

  onSubmit(): void {

    if (
      this.transactionForm.invalid
    ) {

      this.transactionForm
        .markAllAsTouched();

      this.cdr.detectChanges();

      return;

    }


    this.isSubmitting = true;

    this.errorMessage = '';


    const value =
      this.transactionForm.getRawValue();


    const request:
      CreateInventoryTransactionRequest = {

      warehouseId:
        Number(value.warehouseId),

      productId:
        Number(value.productId),

      transactionType:
        Number(
          value.transactionType
        ) as InventoryTransactionType,

      quantity:
        Number(value.quantity),

      referenceNumber:
        value.referenceNumber ||
        undefined,

      remarks:
        value.remarks ||
        undefined,

    };


    this.transactionService
      .create(request)
      .subscribe({

        next: (
          transaction
        ) => {

          this.transactions = [

            transaction,

            ...this.transactions,

          ];


          this.filteredTransactions = [
            ...this.transactions,
          ];


          this.isSubmitting =
            false;

          this.closeForm();

          this.cdr.detectChanges();

        },


        error: (
          error
        ) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to create inventory transaction.';

          this.isSubmitting =
            false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // SEARCH / FILTER
  // =========================

  applyFilters(): void {

    let result =
      [...this.transactions];


    // Warehouse

    if (
      Number(
        this.selectedWarehouseId
      ) > 0
    ) {

      result =
        result.filter(

          transaction =>

            transaction.warehouseId ===
            Number(
              this.selectedWarehouseId
            )

        );

    }


    // Product

    if (
      Number(
        this.selectedProductId
      ) > 0
    ) {

      result =
        result.filter(

          transaction =>

            transaction.productId ===
            Number(
              this.selectedProductId
            )

        );

    }


    // Transaction Type

    if (
      this.selectedTransactionType
    ) {

      result =
        result.filter(

          transaction =>

            transaction.transactionType
              .toLowerCase()
              .replace(/\s/g, '') ===

            this.selectedTransactionType
              .toLowerCase()
              .replace(/\s/g, '')

        );

    }


    // Search

    const search =
      this.searchTerm
        .trim()
        .toLowerCase();


    if (
      search
    ) {

      result =
        result.filter(

          transaction =>

            transaction.productName
              .toLowerCase()
              .includes(search)

            ||

            transaction.sku
              .toLowerCase()
              .includes(search)

            ||

            transaction.warehouseName
              .toLowerCase()
              .includes(search)

            ||

            transaction.referenceNumber
              ?.toLowerCase()
              .includes(search)

        );

    }


    this.filteredTransactions =
      result;

    this.cdr.detectChanges();

  }


  // =========================
  // RESET FILTERS
  // =========================

  resetFilters(): void {

    this.searchTerm = '';

    this.selectedWarehouseId = 0;

    this.selectedProductId = 0;

    this.selectedTransactionType = '';


    this.filteredTransactions = [
      ...this.transactions,
    ];

    this.cdr.detectChanges();

  }


  // =========================
  // REFRESH
  // =========================

  refresh(): void {

    this.resetFilters();

    this.loadInitialData();

  }


  getTransactionTypeClass(
    transactionType: string
  ): string {

    const type =
      transactionType
        .toLowerCase()
        .replace(/\s/g, '');


    if (
      type === 'stockin'
    ) {

      return 'bg-green-100 text-green-700';

    }


    if (
      type === 'stockout'
    ) {

      return'bg-red-100 text-red-700';

    }


    return'bg-orange-100 text-orange-700';

  }

}