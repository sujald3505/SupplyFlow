import {
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { WarehouseService }
  from '../../core/services/warehouse.service';

import {
  Warehouse,
  CreateWarehouseRequest,
  UpdateWarehouseRequest,
} from '../../core/models/warehouse.model';


@Component({
  selector: 'app-warehouses',

  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],

  templateUrl: './warehouses.html',

  styleUrl: './warehouses.css',
})
export class Warehouses implements OnInit {

  warehouses: Warehouse[] = [];

  selectedWarehouse: Warehouse | null = null;

  isLoading = true;

  isSubmitting = false;

  showForm = false;

  errorMessage = '';

  warehouseForm;


  constructor(
    private readonly fb: FormBuilder,
    private readonly warehouseService: WarehouseService,
    private readonly cdr: ChangeDetectorRef
  ) {

    this.warehouseForm =
      this.fb.nonNullable.group({

        name: [
          '',
          [
            Validators.required,
            Validators.maxLength(150),
          ],
        ],

        code: [
          '',
          [
            Validators.maxLength(50),
          ],
        ],

        address: [
          '',
          [
            Validators.maxLength(500),
          ],
        ],

        contactPerson: [
          '',
          [
            Validators.maxLength(100),
          ],
        ],

        phone: [
          '',
          [
            Validators.maxLength(30),
          ],
        ],

        isActive: [
          true,
        ],

      });

  }


  ngOnInit(): void {
    this.loadWarehouses();
  }


  // =========================
  // LOAD WAREHOUSES
  // =========================

  loadWarehouses(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.warehouseService
      .getAll()
      .subscribe({

        next: (response) => {

          this.warehouses = response;

          this.isLoading = false;

          this.cdr.detectChanges();

        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to load warehouses.';

          this.isLoading = false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // OPEN ADD FORM
  // =========================

  openAddForm(): void {

    this.selectedWarehouse = null;

    this.showForm = true;

    this.errorMessage = '';

    this.warehouseForm.reset({

      name: '',

      code: '',

      address: '',

      contactPerson: '',

      phone: '',

      isActive: true,

    });

    this.cdr.detectChanges();

  }


  // =========================
  // OPEN EDIT FORM
  // =========================

  openEditForm(
    warehouse: Warehouse
  ): void {

    this.selectedWarehouse =
      warehouse;

    this.showForm = true;

    this.errorMessage = '';

    this.warehouseForm.patchValue({

      name:
        warehouse.name,

      code:
        warehouse.code ?? '',

      address:
        warehouse.address ?? '',

      contactPerson:
        warehouse.contactPerson ?? '',

      phone:
        warehouse.phone ?? '',

      isActive:
        warehouse.isActive,

    });

    this.cdr.detectChanges();

  }


  // =========================
  // CLOSE FORM
  // =========================

  closeForm(): void {

    this.showForm = false;

    this.selectedWarehouse = null;

    this.warehouseForm.reset();

    this.cdr.detectChanges();

  }


  // =========================
  // SUBMIT
  // =========================

  onSubmit(): void {

    if (
      this.warehouseForm.invalid
    ) {

      this.warehouseForm
        .markAllAsTouched();

      return;

    }

    this.isSubmitting = true;

    this.errorMessage = '';

    if (
      this.selectedWarehouse
    ) {

      this.updateWarehouse();

    } else {

      this.createWarehouse();

    }

  }


  // =========================
  // CREATE WAREHOUSE
  // =========================

  private createWarehouse(): void {

    const formValue =
      this.warehouseForm.getRawValue();

    const request:
      CreateWarehouseRequest = {

      name:
        formValue.name,

      code:
        formValue.code || undefined,

      address:
        formValue.address || undefined,

      contactPerson:
        formValue.contactPerson || undefined,

      phone:
        formValue.phone || undefined,

    };


    this.warehouseService
      .create(request)
      .subscribe({

        next: (
          warehouse
        ) => {

          this.warehouses = [

            ...this.warehouses,

            warehouse,

          ];

          this.isSubmitting = false;

          this.closeForm();

          this.cdr.detectChanges();

        },

        error: (
          error
        ) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to create warehouse.';

          this.isSubmitting = false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // UPDATE WAREHOUSE
  // =========================

  private updateWarehouse(): void {

    if (
      !this.selectedWarehouse
    ) {
      return;
    }


    const formValue =
      this.warehouseForm.getRawValue();


    const request:
      UpdateWarehouseRequest = {

      name:
        formValue.name,

      code:
        formValue.code || undefined,

      address:
        formValue.address || undefined,

      contactPerson:
        formValue.contactPerson || undefined,

      phone:
        formValue.phone || undefined,

      isActive:
        formValue.isActive,

    };


    this.warehouseService
      .update(

        this.selectedWarehouse.id,

        request

      )
      .subscribe({

        next: (
          updatedWarehouse
        ) => {

          this.warehouses =
            this.warehouses.map(

              warehouse =>

                warehouse.id ===
                updatedWarehouse.id

                  ? updatedWarehouse

                  : warehouse

            );


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
            'Failed to update warehouse.';

          this.isSubmitting =
            false;

          this.cdr.detectChanges();

        },

      });

  }

}