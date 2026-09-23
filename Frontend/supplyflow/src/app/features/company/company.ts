import {
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { CommonModule } from '@angular/common';

import {
  Company,
  UpdateCompanyRequest,
} from '../../core/models/company.model';

import { CompanyService } from '../../core/services/company.service';

@Component({
  selector: 'app-company',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './company.html',
  styleUrl: './company.css',
})
export class CompanyComponent implements OnInit {

  company: Company | null = null;

  isLoading = false;
  isSaving = false;

  successMessage = '';
  errorMessage = '';

  companyForm;

  constructor(
    private readonly fb: FormBuilder,
    private readonly companyService: CompanyService,
    private readonly cdr: ChangeDetectorRef
  ) {
    this.companyForm = this.fb.nonNullable.group({
      name: [
        '',
        [
          Validators.required,
          Validators.maxLength(150),
        ],
      ],

      email: [
        '',
        [
          Validators.required,
          Validators.email,
        ],
      ],

      phone: [''],

      address: [''],

      gstNumber: [''],
    });
  }

  ngOnInit(): void {
    this.loadCompany();
  }

  loadCompany(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.companyService
      .getCurrentCompany()
      .subscribe({
        next: (company) => {
          this.company = company;

          this.companyForm.patchValue({
            name: company.name,
            email: company.email,
            phone: company.phone ?? '',
            address: company.address ?? '',
            gstNumber: company.gstNumber ?? '',
          });

          this.isLoading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Failed to load company details.';

          this.cdr.detectChanges();
        },
      });
  }

  onSubmit(): void {

    this.successMessage = '';
    this.errorMessage = '';

    if (this.companyForm.invalid) {
      this.companyForm.markAllAsTouched();

      this.errorMessage =
        'Please fill all required fields correctly.';

      this.cdr.detectChanges();
      return;
    }

    this.isSaving = true;

    const formValue =
      this.companyForm.getRawValue();

    const request: UpdateCompanyRequest = {
      name: formValue.name,
      email: formValue.email,
      phone: formValue.phone || null,
      address: formValue.address || null,
      gstNumber: formValue.gstNumber || null,
    };

    this.companyService
      .updateCompany(request)
      .subscribe({
        next: (company) => {
          this.company = company;

          this.isSaving = false;

          this.successMessage =
            'Company details updated successfully.';

          this.companyForm.patchValue({
            name: company.name,
            email: company.email,
            phone: company.phone ?? '',
            address: company.address ?? '',
            gstNumber: company.gstNumber ?? '',
          });

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.isSaving = false;

          this.errorMessage =
            error?.error?.message ||
            'Failed to update company details.';

          this.cdr.detectChanges();
        },
      });
  }

  get name() {
    return this.companyForm.controls.name;
  }

  get email() {
    return this.companyForm.controls.email;
  }

}