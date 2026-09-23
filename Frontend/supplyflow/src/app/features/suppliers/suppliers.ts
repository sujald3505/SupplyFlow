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

import { SupplierService }
  from '../../core/services/supplier.service';

import {
  Supplier,
  CreateSupplierRequest,
  UpdateSupplierRequest,
} from '../../core/models/supplier.model';


@Component({
  selector: 'app-suppliers',

  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],

  templateUrl: './suppliers.html',

  styleUrl: './suppliers.css',
})
export class Suppliers implements OnInit {

  suppliers: Supplier[] = [];

  selectedSupplier: Supplier | null = null;

  isLoading = true;

  isSubmitting = false;

  showForm = false;

  errorMessage = '';

  supplierForm;


  constructor(
    private readonly fb: FormBuilder,
    private readonly supplierService: SupplierService,
    private readonly cdr: ChangeDetectorRef
  ) {

    this.supplierForm =
      this.fb.nonNullable.group({

        name: [
          '',
          [
            Validators.required,
            Validators.maxLength(150),
          ],
        ],

        contactPerson: [
          '',
          [
            Validators.required,
            Validators.maxLength(150),
          ],
        ],

        email: [
          '',
          [
            Validators.email,
            Validators.maxLength(150),
          ],
        ],

        phone: [
          '',
          [
            Validators.maxLength(30),
          ],
        ],

        address: [
          '',
          [
            Validators.maxLength(500),
          ],
        ],

        gstNumber: [
          '',
          [
            Validators.maxLength(50),
          ],
        ],

        isActive: [
          true,
        ],

      });

  }


  ngOnInit(): void {
    this.loadSuppliers();
  }


  // =========================
  // LOAD SUPPLIERS
  // =========================

  loadSuppliers(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.supplierService
      .getAll()
      .subscribe({

        next: (response) => {

          this.suppliers = response;

          this.isLoading = false;

          this.cdr.detectChanges();

        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to load suppliers.';

          this.isLoading = false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // OPEN ADD FORM
  // =========================

  openAddForm(): void {

    this.selectedSupplier = null;

    this.showForm = true;

    this.errorMessage = '';

    this.supplierForm.reset({

      name: '',

      contactPerson: '',

      email: '',

      phone: '',

      address: '',

      gstNumber: '',

      isActive: true,

    });

    this.cdr.detectChanges();

  }


  // =========================
  // OPEN EDIT FORM
  // =========================

  openEditForm(
    supplier: Supplier
  ): void {

    this.selectedSupplier = supplier;

    this.showForm = true;

    this.errorMessage = '';

    this.supplierForm.patchValue({

      name:
        supplier.name,

      contactPerson:
        supplier.contactPerson,

      email:
        supplier.email ?? '',

      phone:
        supplier.phone ?? '',

      address:
        supplier.address ?? '',

      gstNumber:
        supplier.gstNumber ?? '',

      isActive:
        supplier.isActive,

    });

    this.cdr.detectChanges();

  }


  // =========================
  // CLOSE FORM
  // =========================

  closeForm(): void {

    this.showForm = false;

    this.selectedSupplier = null;

    this.supplierForm.reset();

    this.cdr.detectChanges();

  }


  // =========================
  // SUBMIT
  // =========================

  onSubmit(): void {

    if (
      this.supplierForm.invalid
    ) {

      this.supplierForm
        .markAllAsTouched();

      return;

    }

    this.isSubmitting = true;

    this.errorMessage = '';

    if (
      this.selectedSupplier
    ) {

      this.updateSupplier();

    } else {

      this.createSupplier();

    }

  }


  // =========================
  // CREATE SUPPLIER
  // =========================

  private createSupplier(): void {

    const formValue =
      this.supplierForm.getRawValue();

    const request:
      CreateSupplierRequest = {

      name:
        formValue.name,

      contactPerson:
        formValue.contactPerson,

      email:
        formValue.email || undefined,

      phone:
        formValue.phone || undefined,

      address:
        formValue.address || undefined,

      gstNumber:
        formValue.gstNumber || undefined,

    };


    this.supplierService
      .create(request)
      .subscribe({

        next: (
          supplier
        ) => {

          this.suppliers = [

            ...this.suppliers,

            supplier,

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
            'Failed to create supplier.';

          this.isSubmitting = false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // UPDATE SUPPLIER
  // =========================

  private updateSupplier(): void {

    if (
      !this.selectedSupplier
    ) {
      return;
    }


    const formValue =
      this.supplierForm.getRawValue();


    const request:
      UpdateSupplierRequest = {

      name:
        formValue.name,

      contactPerson:
        formValue.contactPerson,

      email:
        formValue.email || undefined,

      phone:
        formValue.phone || undefined,

      address:
        formValue.address || undefined,

      gstNumber:
        formValue.gstNumber || undefined,

      isActive:
        formValue.isActive,

    };


    this.supplierService
      .update(

        this.selectedSupplier.id,

        request

      )
      .subscribe({

        next: (
          updatedSupplier
        ) => {

          this.suppliers =
            this.suppliers.map(

              supplier =>

                supplier.id ===
                updatedSupplier.id

                  ? updatedSupplier

                  : supplier

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
            'Failed to update supplier.';

          this.isSubmitting =
            false;

          this.cdr.detectChanges();

        },

      });

  }

}