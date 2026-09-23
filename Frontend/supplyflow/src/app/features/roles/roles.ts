import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { CommonModule } from '@angular/common';

import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Role, CreateRoleRequest } from '../../core/models/role.model';

import { RoleService } from '../../core/services/role.service';

@Component({
  selector: 'app-roles',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './roles.html',
  styleUrl: './roles.css',
})
export class Roles implements OnInit {
  roles: Role[] = [];

  isLoading = false;
  isSaving = false;

  showForm = false;

  successMessage = '';
  errorMessage = '';

  roleForm;

  constructor(
    private readonly fb: FormBuilder,
    private readonly roleService: RoleService,
    private readonly cdr: ChangeDetectorRef,
  ) {
    this.roleForm = this.fb.nonNullable.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],

      description: [''],
    });
  }

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.roleService.getAll().subscribe({
      next: (roles) => {
        this.roles = roles;

        this.isLoading = false;

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.isLoading = false;

        this.errorMessage = error?.error?.message || 'Failed to load roles.';

        this.cdr.detectChanges();
      },
    });
  }

  openAddForm(): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.roleForm.reset({
      name: '',
      description: '',
    });

    this.showForm = true;

    this.cdr.detectChanges();
  }

  closeForm(): void {
    this.showForm = false;

    this.roleForm.reset({
      name: '',
      description: '',
    });

    this.cdr.detectChanges();
  }

  onSubmit(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.roleForm.invalid) {
      this.roleForm.markAllAsTouched();

      this.errorMessage = 'Please enter a valid role name.';

      this.cdr.detectChanges();
      return;
    }

    this.isSaving = true;

    const formValue = this.roleForm.getRawValue();

    const request: CreateRoleRequest = {
      name: formValue.name.trim(),
      description: formValue.description.trim() || null,
    };

    this.roleService.create(request).subscribe({
      next: (role) => {
        // New role UI માં તરત add થશે
        this.roles = [...this.roles, role];

        this.isSaving = false;
        this.showForm = false;

        this.successMessage = 'Role created successfully.';

        this.roleForm.reset({
          name: '',
          description: '',
        });

        this.cdr.detectChanges();
      },

      error: (error) => {
        this.isSaving = false;

        this.errorMessage = error?.error?.message || 'Failed to create role.';

        this.cdr.detectChanges();
      },
    });
  }

  get name() {
    return this.roleForm.controls.name;
  }
}
