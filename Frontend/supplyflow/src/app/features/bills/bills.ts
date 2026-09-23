import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { SaleService } from '../../core/services/sale.service';
import { CustomerService } from '../../core/services/customer.service';

import { Sale } from '../../core/models/sale.model';
import { Customer } from '../../core/models/customer.model';

@Component({
  selector: 'app-bill',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './bills.html',
  styleUrl: './bills.css',
})
export class Bill implements OnInit {

  sale: Sale | null = null;

  customer: Customer | null = null;

  isLoading = true;

  errorMessage = '';

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly saleService: SaleService,
    private readonly customerService: CustomerService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {

    const id = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!id || id <= 0) {

      this.errorMessage = 'Invalid invoice ID.';

      this.isLoading = false;

      this.cdr.detectChanges();

      return;
    }

    this.loadBill(id);
  }

  loadBill(id: number): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.cdr.detectChanges();

    this.saleService.getById(id).subscribe({

      next: (sale: Sale) => {

        this.sale = sale;

        /*
         * Sale successfully loaded.
         * Stop loading immediately.
         */
        this.isLoading = false;

        this.cdr.detectChanges();


        /*
         * Customer details are additional information.
         * Even if customer API fails,
         * invoice should still be displayed.
         */
        this.customerService
          .getById(sale.customerId)
          .subscribe({

            next: (customer: Customer) => {

              this.customer = customer;

              this.cdr.detectChanges();
            },

            error: (error) => {

              console.error(
                'Failed to load customer:',
                error
              );

              this.customer = null;

              this.cdr.detectChanges();
            },
          });
      },

      error: (error) => {

        console.error(
          'Failed to load sale:',
          error
        );

        this.errorMessage =
          error?.error?.message ??
          'Failed to load invoice.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  printBill(): void {

    window.print();
  }

  goBack(): void {

    this.router.navigate(['/sales']);
  }
}