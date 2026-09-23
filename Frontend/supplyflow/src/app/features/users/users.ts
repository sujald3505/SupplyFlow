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

import {
  User,
  CreateUserRequest,
  UpdateUserRequest,
} from '../../core/models/user.model';

import { Role } from '../../core/models/role.model';

import { UserService } from '../../core/services/user.service';
import { RoleService } from '../../core/services/role.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
  ],
  templateUrl: './users.html',
  styleUrl: './users.css',
})
export class Users implements OnInit {

  users: User[] = [];
  roles: Role[] = [];

  isLoading = false;
  isSaving = false;
  isLoadingRoles = false;

  showForm = false;
  isEditMode = false;

  selectedUserId: number | null = null;

  successMessage = '';
  errorMessage = '';

  userForm;

  constructor(
    private readonly fb: FormBuilder,
    private readonly userService: UserService,
    private readonly roleService: RoleService,
    private readonly cdr: ChangeDetectorRef
  ) {
    this.userForm = this.fb.nonNullable.group({
      fullName: [
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

      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
        ],
      ],

      phone: [''],

      roleId: [
        0,
        [
          Validators.required,
          Validators.min(1),
        ],
      ],

      isActive: [true],
    });
  }


  ngOnInit(): void {
    this.loadUsers();
    this.loadRoles();
  }


  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.userService
      .getAll()
      .subscribe({
        next: (users) => {
          this.users = users;

          this.isLoading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ||
            'Failed to load users.';

          this.cdr.detectChanges();
        },
      });
  }


  loadRoles(): void {
    this.isLoadingRoles = true;

    this.roleService
      .getAll()
      .subscribe({
        next: (roles) => {
          this.roles = roles.filter(
            (role) => role.isActive
          );

          this.isLoadingRoles = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.isLoadingRoles = false;

          this.errorMessage =
            error?.error?.message ||
            'Failed to load roles.';

          this.cdr.detectChanges();
        },
      });
  }


  openAddForm(): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.isEditMode = false;
    this.selectedUserId = null;

    this.userForm.reset({
      fullName: '',
      email: '',
      password: '',
      phone: '',
      roleId: 0,
      isActive: true,
    });

    // Create mode માં Email અને Password editable
    this.userForm.controls.email.enable();
    this.userForm.controls.password.enable();

    // Password required છે create mode માં
    this.userForm.controls.password.setValidators([
      Validators.required,
      Validators.minLength(6),
    ]);

    this.userForm.controls.password.updateValueAndValidity();

    this.showForm = true;

    this.cdr.detectChanges();
  }


  openEditForm(user: User): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.isEditMode = true;
    this.selectedUserId = user.id;

    this.userForm.patchValue({
      fullName: user.fullName,
      email: user.email,
      password: '',
      phone: user.phone ?? '',
      roleId: user.roleId,
      isActive: user.isActive,
    });

    // Backend UpdateUserDto માં Email નથી
    this.userForm.controls.email.disable();

    // Backend UpdateUserDto માં Password નથી
    this.userForm.controls.password.clearValidators();
    this.userForm.controls.password.setValue('');
    this.userForm.controls.password.disable();
    this.userForm.controls.password.updateValueAndValidity();

    this.showForm = true;

    this.cdr.detectChanges();
  }


  closeForm(): void {
    this.showForm = false;

    this.isEditMode = false;
    this.selectedUserId = null;

    this.userForm.reset({
      fullName: '',
      email: '',
      password: '',
      phone: '',
      roleId: 0,
      isActive: true,
    });

    this.userForm.controls.email.enable();
    this.userForm.controls.password.enable();

    this.cdr.detectChanges();
  }


  onSubmit(): void {

    this.successMessage = '';
    this.errorMessage = '';

    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();

      this.errorMessage =
        'Please fill all required fields correctly.';

      this.cdr.detectChanges();
      return;
    }

    this.isSaving = true;

    const formValue =
      this.userForm.getRawValue();

    if (this.isEditMode &&
        this.selectedUserId !== null) {

      const request: UpdateUserRequest = {
        fullName: formValue.fullName.trim(),
        phone: formValue.phone.trim() || null,
        roleId: formValue.roleId,
        isActive: formValue.isActive,
      };

      this.updateUser(
        this.selectedUserId,
        request
      );

      return;
    }


    const request: CreateUserRequest = {
      fullName: formValue.fullName.trim(),
      email: formValue.email.trim(),
      password: formValue.password,
      phone: formValue.phone.trim() || null,
      roleId: formValue.roleId,
    };

    this.createUser(request);
  }


  private createUser(
    request: CreateUserRequest
  ): void {

    this.userService
      .create(request)
      .subscribe({
        next: (user) => {

          this.users = [
            ...this.users,
            user,
          ];

          this.isSaving = false;
          this.showForm = false;

          this.successMessage =
            'User created successfully.';

          this.resetAfterSave();

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.isSaving = false;

          this.errorMessage =
            error?.error?.message ||
            'Failed to create user.';

          this.cdr.detectChanges();
        },
      });
  }


  private updateUser(
    id: number,
    request: UpdateUserRequest
  ): void {

    this.userService
      .update(id, request)
      .subscribe({
        next: (updatedUser) => {

          this.users = this.users.map(
            (user) =>
              user.id === updatedUser.id
                ? updatedUser
                : user
          );

          this.isSaving = false;
          this.showForm = false;

          this.successMessage =
            'User updated successfully.';

          this.resetAfterSave();

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.isSaving = false;

          this.errorMessage =
            error?.error?.message ||
            'Failed to update user.';

          this.cdr.detectChanges();
        },
      });
  }


  private resetAfterSave(): void {

    this.isEditMode = false;
    this.selectedUserId = null;

    this.userForm.reset({
      fullName: '',
      email: '',
      password: '',
      phone: '',
      roleId: 0,
      isActive: true,
    });

    this.userForm.controls.email.enable();
    this.userForm.controls.password.enable();

    this.userForm.controls.password.setValidators([
      Validators.required,
      Validators.minLength(6),
    ]);

    this.userForm.controls.password.updateValueAndValidity();
  }


  get fullName() {
    return this.userForm.controls.fullName;
  }

  get email() {
    return this.userForm.controls.email;
  }

  get password() {
    return this.userForm.controls.password;
  }

  get roleId() {
    return this.userForm.controls.roleId;
  }
}