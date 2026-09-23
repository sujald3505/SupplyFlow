import {ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { CategoryService } from '../../core/services/category.service';
import {
  Category,
  CreateCategoryRequest,
} from '../../core/models/category.model';

@Component({
  selector: 'app-categories',
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './categories.html',
  styleUrl: './categories.css',
})
export class Categories implements OnInit {
  categories: Category[] = [];

  isLoading = true;
  isSubmitting = false;
  showForm = false;

  errorMessage = '';

  categoryForm;

  constructor(
    private readonly fb: FormBuilder,
    private readonly categoryService: CategoryService,
    private readonly cdr: ChangeDetectorRef
  ) {
    this.categoryForm =
      this.fb.nonNullable.group({
        name: [
          '',
          [
            Validators.required,
            Validators.maxLength(100),
          ],
        ],

        description: [
          '',
          [
            Validators.maxLength(500),
          ],
        ],
      });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
  this.isLoading = true;
  this.errorMessage = '';

  this.categoryService
    .getAll()
    .subscribe({
      next: (response) => {
        this.categories = response;

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage =
          error?.error?.message ??
          'Failed to load categories.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
}

  openForm(): void {
    this.showForm = true;
    this.categoryForm.reset();
    this.errorMessage = '';
  }

  closeForm(): void {
    this.showForm = false;
    this.categoryForm.reset();
  }

  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    const request: CreateCategoryRequest =
      this.categoryForm.getRawValue();

    this.categoryService
      .create(request)
      .subscribe({
        next: (category) => {
          this.categories = [
            ...this.categories,
            category,
          ];

          this.isSubmitting = false;

          this.closeForm();

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.errorMessage =
            error?.error?.message ??
            'Failed to create category.';

          this.isSubmitting = false;
        },
      });
  }
}