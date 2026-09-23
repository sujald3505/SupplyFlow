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

import { UnitService } from '../../core/services/unit.service';

import {
  Unit,
  CreateUnitRequest,
  UpdateUnitRequest,
} from '../../core/models/unit.model';

@Component({
  selector: 'app-units',
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './units.html',
  styleUrl: './units.css',
})
export class Units implements OnInit {

  units: Unit[] = [];

  selectedUnit: Unit | null = null;

  isLoading = true;

  isSubmitting = false;

  showForm = false;

  errorMessage = '';

  unitForm;

  constructor(
    private readonly fb: FormBuilder,
    private readonly unitService: UnitService,
    private readonly cdr: ChangeDetectorRef
  ) {

    this.unitForm =
      this.fb.nonNullable.group({

        name: [
          '',
          [
            Validators.required,
            Validators.maxLength(100),
          ],
        ],

        symbol: [
          '',
          [
            Validators.required,
            Validators.maxLength(20),
          ],
        ],

        description: [
          '',
          [
            Validators.maxLength(500),
          ],
        ],

        isActive: [
          true,
        ],

      });

  }


  ngOnInit(): void {
    this.loadUnits();
  }


  // =========================
  // LOAD UNITS
  // =========================

  loadUnits(): void {

    this.isLoading = true;

    this.errorMessage = '';

    this.unitService
      .getAll()
      .subscribe({

        next: (response) => {

          this.units = response;

          this.isLoading = false;

          this.cdr.detectChanges();

        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to load units.';

          this.isLoading = false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // OPEN ADD FORM
  // =========================

  openAddForm(): void {

    this.selectedUnit = null;

    this.showForm = true;

    this.unitForm.reset({
      name: '',
      symbol: '',
      description: '',
      isActive: true,
    });

    this.errorMessage = '';

    this.cdr.detectChanges();

  }


  // =========================
  // OPEN EDIT FORM
  // =========================

  openEditForm(unit: Unit): void {

    this.selectedUnit = unit;

    this.showForm = true;

    this.errorMessage = '';

    this.unitForm.patchValue({

      name: unit.name,

      symbol: unit.symbol,

      description: unit.description ?? '',

      isActive: unit.isActive,

    });

    this.cdr.detectChanges();

  }


  // =========================
  // CLOSE FORM
  // =========================

  closeForm(): void {

    this.showForm = false;

    this.selectedUnit = null;

    this.unitForm.reset();

    this.cdr.detectChanges();

  }


  // =========================
  // SUBMIT
  // =========================

  onSubmit(): void {

    if (this.unitForm.invalid) {

      this.unitForm.markAllAsTouched();

      return;

    }

    this.isSubmitting = true;

    this.errorMessage = '';

    if (this.selectedUnit) {

      this.updateUnit();

    } else {

      this.createUnit();

    }

  }


  // =========================
  // CREATE UNIT
  // =========================

  private createUnit(): void {

    const formValue =
      this.unitForm.getRawValue();

    const request: CreateUnitRequest = {

      name: formValue.name,

      symbol: formValue.symbol,

      description:
        formValue.description || undefined,

    };

    this.unitService
      .create(request)
      .subscribe({

        next: (unit) => {

          this.units = [
            ...this.units,
            unit,
          ];

          this.isSubmitting = false;

          this.closeForm();

          this.cdr.detectChanges();

        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to create unit.';

          this.isSubmitting = false;

          this.cdr.detectChanges();

        },

      });

  }


  // =========================
  // UPDATE UNIT
  // =========================

  private updateUnit(): void {

    if (!this.selectedUnit) {
      return;
    }

    const formValue =
      this.unitForm.getRawValue();

    const request: UpdateUnitRequest = {

      name: formValue.name,

      symbol: formValue.symbol,

      description:
        formValue.description || undefined,

      isActive:
        formValue.isActive,

    };

    this.unitService
      .update(
        this.selectedUnit.id,
        request
      )
      .subscribe({

        next: (updatedUnit) => {

          this.units =
            this.units.map(unit =>
              unit.id === updatedUnit.id
                ? updatedUnit
                : unit
            );

          this.isSubmitting = false;

          this.closeForm();

          this.cdr.detectChanges();

        },

        error: (error) => {

          this.errorMessage =
            error?.error?.message ??
            'Failed to update unit.';

          this.isSubmitting = false;

          this.cdr.detectChanges();

        },

      });

  }

}