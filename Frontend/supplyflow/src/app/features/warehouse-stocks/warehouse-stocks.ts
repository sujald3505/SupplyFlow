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
} from '@angular/forms';

import {
  forkJoin,
} from 'rxjs';

import {
  WarehouseStockService,
} from '../../core/services/warehouse-stock.service';

import {
  WarehouseStock,
} from '../../core/models/warehouse-stock.model';

import {
  WarehouseService,
} from '../../core/services/warehouse.service';


@Component({
  selector: 'app-warehouse-stocks',

  imports: [
    CommonModule,
    FormsModule,
  ],

  templateUrl:
    './warehouse-stocks.html',

  styleUrl:
    './warehouse-stocks.css',
})
export class WarehouseStocks
  implements OnInit {


  stocks: WarehouseStock[] = [];

  filteredStocks:
    WarehouseStock[] = [];

  warehouses: any[] = [];


  searchTerm = '';

  selectedWarehouseId = 0;

  lowStockOnly = false;


  isLoading = true;

  errorMessage = '';


  constructor(

    private readonly stockService:
      WarehouseStockService,

    private readonly warehouseService:
      WarehouseService,

    private readonly cdr:
      ChangeDetectorRef

  ) {}


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

      stocks:
        this.stockService.getAll(),

      warehouses:
        this.warehouseService.getAll(),

    })
      .subscribe({

        next: (response) => {

          this.stocks =
            response.stocks;

          this.filteredStocks =
            response.stocks;

          this.warehouses =
            response.warehouses;

          this.isLoading =
            false;

          this.cdr.detectChanges();

        },


        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to load warehouse stock.';

          this.isLoading =
            false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // SEARCH
  // =========================

  onSearchChange(): void {

    this.applyFilters();

  }


  // =========================
  // WAREHOUSE FILTER
  // =========================

  onWarehouseChange(): void {

    this.applyFilters();

  }


  // =========================
  // LOW STOCK FILTER
  // =========================

  toggleLowStock(): void {

  if (this.lowStockOnly) {

    this.lowStockOnly = false;

    this.applyFilters();

    return;

  }


  this.showLowStock();

}


  // =========================
  // APPLY FILTERS
  // =========================

  applyFilters(): void {

    let result =
      [...this.stocks];


    // Warehouse Filter

    if (
      this.selectedWarehouseId > 0
    ) {

      result =
        result.filter(

          stock =>

            stock.warehouseId ===
            Number(
              this.selectedWarehouseId
            )

        );

    }


    // Search Filter

    const search =
      this.searchTerm
        .trim()
        .toLowerCase();


    if (
      search
    ) {

      result =
        result.filter(

          stock =>

            stock.productName
              .toLowerCase()
              .includes(search)

            ||

            stock.sku
              .toLowerCase()
              .includes(search)

            ||

            stock.warehouseName
              .toLowerCase()
              .includes(search)

        );

    }


    this.filteredStocks =
      result;

    this.cdr.detectChanges();

  }


  // =========================
  // RESET FILTERS
  // =========================

  resetFilters(): void {

    this.searchTerm =
      '';

    this.selectedWarehouseId =
      0;

    this.lowStockOnly =
      false;

    this.filteredStocks =
      [...this.stocks];

    this.cdr.detectChanges();

  }


  // =========================
  // LOW STOCK
  // =========================

  showLowStock(): void {

    this.isLoading =
      true;

    this.errorMessage =
      '';


    this.stockService
      .getLowStock()
      .subscribe({

        next: (
          response
        ) => {

          /*
           LowStockDto can be different
           from WarehouseStockDto.

           If your backend returns the same
           structure this will work directly.
          */

          this.filteredStocks =
            response as WarehouseStock[];

          this.lowStockOnly =
            true;

          this.isLoading =
            false;

          this.cdr.detectChanges();

        },


        error: (
          error
        ) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to load low stock items.';

          this.isLoading =
            false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // REFRESH
  // =========================

  refresh(): void {

    this.resetFilters();

    this.loadInitialData();

  }

}