import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { forkJoin } from 'rxjs';

import { ProductService } from '../../core/services/product.service';

import { CategoryService } from '../../core/services/category.service';

import { UnitService } from '../../core/services/unit.service';

import {
  Product,
  CreateProductRequest,
  UpdateProductRequest,
} from '../../core/models/product.model';

@Component({
  selector: 'app-products',

  imports: [CommonModule, ReactiveFormsModule],

  templateUrl: './products.html',

  styleUrl: './products.css',
})
export class Products implements OnInit {
  products: Product[] = [];

  categories: any[] = [];

  units: any[] = [];

  selectedProduct: Product | null = null;

  isLoading = true;

  isSubmitting = false;

  showForm = false;

  errorMessage = '';

  // =========================
  // IMAGE
  // =========================

  selectedImage: File | null = null;

  imagePreview: string | null = null;

  isUploadingImage = false;

  productForm;

  constructor(
    private readonly fb: FormBuilder,

    private readonly productService: ProductService,

    private readonly categoryService: CategoryService,

    private readonly unitService: UnitService,

    private readonly cdr: ChangeDetectorRef,
  ) {
    this.productForm = this.fb.nonNullable.group({
      categoryId: [0, Validators.min(1)],

      unitId: [0, Validators.min(1)],

      name: ['', [Validators.required, Validators.maxLength(150)]],

      sku: ['', [Validators.required, Validators.maxLength(100)]],

      description: ['', Validators.maxLength(500)],

      costPrice: [0, [Validators.required, Validators.min(0)]],

      sellingPrice: [0, [Validators.required, Validators.min(0)]],

      minimumStockLevel: [0, [Validators.required, Validators.min(0)]],

      isActive: [true],
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
      products: this.productService.getAll(),

      categories: this.categoryService.getAll(),

      units: this.unitService.getAll(),
    }).subscribe({
      next: (response) => {
        this.products = response.products;

        this.categories = response.categories;

        this.units = response.units;

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message ?? 'Failed to load products.';

        this.isLoading = false;

        this.cdr.detectChanges();
      },
    });
  }

  // =========================
  // OPEN ADD FORM
  // =========================

  openAddForm(): void {
    this.selectedProduct = null;

    this.showForm = true;

    this.errorMessage = '';

    this.resetImage();

    this.productForm.reset({
      categoryId: 0,

      unitId: 0,

      name: '',

      sku: '',

      description: '',

      costPrice: 0,

      sellingPrice: 0,

      minimumStockLevel: 0,

      isActive: true,
    });

    this.cdr.detectChanges();
  }

  // =========================
  // OPEN EDIT FORM
  // =========================

  openEditForm(product: Product): void {
    this.selectedProduct = product;

    this.showForm = true;

    this.errorMessage = '';

    this.selectedImage = null;

    this.imagePreview = product.imageUrl ? this.getImageUrl(product.imageUrl) : null;

    this.productForm.patchValue({
      categoryId: product.categoryId,

      unitId: product.unitId,

      name: product.name,

      sku: product.sku,

      description: product.description ?? '',

      costPrice: product.costPrice,

      sellingPrice: product.sellingPrice,

      minimumStockLevel: product.minimumStockLevel,

      isActive: product.isActive,
    });

    this.cdr.detectChanges();
  }

  // =========================
  // CLOSE FORM
  // =========================

  closeForm(): void {
    this.showForm = false;

    this.selectedProduct = null;

    this.resetImage();

    this.productForm.reset();

    this.cdr.detectChanges();
  }

  // =========================
  // IMAGE SELECT
  // =========================

  onImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (!input.files || input.files.length === 0) {
      return;
    }

    const file = input.files[0];

    // Allowed types
    const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];

    if (!allowedTypes.includes(file.type)) {
      this.errorMessage = 'Only JPG, PNG and WEBP images are allowed.';

      input.value = '';

      return;
    }

    // 5 MB
    if (file.size > 5 * 1024 * 1024) {
      this.errorMessage = 'Image size must be less than 5 MB.';

      input.value = '';

      return;
    }

    this.errorMessage = '';

    this.selectedImage = file;

    const reader = new FileReader();

    reader.onload = () => {
      this.imagePreview = reader.result as string;

      this.cdr.detectChanges();
    };

    reader.readAsDataURL(file);
  }

  // =========================
  // REMOVE SELECTED IMAGE
  // =========================

  removeImage(): void {
    this.selectedImage = null;

    this.imagePreview = this.selectedProduct?.imageUrl
      ? this.getImageUrl(this.selectedProduct.imageUrl)
      : null;
  }

  // =========================
  // RESET IMAGE
  // =========================

  private resetImage(): void {
    this.selectedImage = null;

    this.imagePreview = null;
  }

  // =========================
  // IMAGE URL
  // =========================

  getImageUrl(imageUrl?: string): string {
    if (!imageUrl) {
      return '';
    }

    if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
      return imageUrl;
    }

    return `https://localhost:7198${imageUrl}`;
  }

  // =========================
  // SUBMIT
  // =========================

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();

      return;
    }

    this.isSubmitting = true;

    this.errorMessage = '';

    if (this.selectedProduct) {
      this.updateProduct();
    } else {
      this.createProduct();
    }
  }

  // =========================
  // CREATE PRODUCT
  // =========================

  private createProduct(): void {
    const value = this.productForm.getRawValue();

    const request: CreateProductRequest = {
      categoryId: value.categoryId,

      unitId: value.unitId,

      name: value.name,

      sku: value.sku,

      description: value.description || undefined,

      costPrice: value.costPrice,

      sellingPrice: value.sellingPrice,

      minimumStockLevel: value.minimumStockLevel,
    };

    this.productService.create(request).subscribe({
      next: (product) => {
        // Add product first
        this.products = [...this.products, product];

        // Upload image if selected
        if (this.selectedImage) {
          this.uploadProductImage(product.id, product);
        } else {
          this.isSubmitting = false;

          this.closeForm();

          this.cdr.detectChanges();
        }
      },

      error: (error) => {
        this.errorMessage = error?.error?.message ?? 'Failed to create product.';

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }

  // =========================
  // UPDATE PRODUCT
  // =========================

  private updateProduct(): void {
    if (!this.selectedProduct) {
      return;
    }

    const value = this.productForm.getRawValue();

    const request: UpdateProductRequest = {
      categoryId: value.categoryId,

      unitId: value.unitId,

      name: value.name,

      sku: value.sku,

      description: value.description || undefined,

      costPrice: value.costPrice,

      sellingPrice: value.sellingPrice,

      minimumStockLevel: value.minimumStockLevel,

      isActive: value.isActive,
    };

    this.productService
      .update(
        this.selectedProduct.id,

        request,
      )
      .subscribe({
        next: (updatedProduct) => {
          this.products = this.products.map((product) =>
            product.id === updatedProduct.id ? updatedProduct : product,
          );

          // Upload new image if selected
          if (this.selectedImage) {
            this.uploadProductImage(updatedProduct.id, updatedProduct);
          } else {
            this.isSubmitting = false;

            this.closeForm();

            this.cdr.detectChanges();
          }
        },

        error: (error) => {
          this.errorMessage = error?.error?.message ?? 'Failed to update product.';

          this.isSubmitting = false;

          this.cdr.detectChanges();
        },
      });
  }

  // =========================
  // UPLOAD IMAGE
  // =========================

  private uploadProductImage(productId: number, product: Product): void {
    if (!this.selectedImage) {
      this.isSubmitting = false;

      this.closeForm();

      return;
    }

    this.isUploadingImage = true;

    this.productService.uploadImage(productId, this.selectedImage).subscribe({
      next: (response) => {
        const imageUrl = response.imageUrl;

        this.products = this.products.map((item) =>
          item.id === productId
            ? {
                ...item,
                imageUrl: imageUrl,
              }
            : item,
        );

        this.isUploadingImage = false;

        this.isSubmitting = false;

        this.closeForm();

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.errorMessage = error?.error?.message ?? 'Product saved, but image upload failed.';

        this.isUploadingImage = false;

        this.isSubmitting = false;

        this.cdr.detectChanges();
      },
    });
  }
}
