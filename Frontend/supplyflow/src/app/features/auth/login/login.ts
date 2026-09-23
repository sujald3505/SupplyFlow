import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import {
  LoginRequest,
} from '../../../core/models/auth.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  isLoading = false;
  errorMessage = '';

  loginForm;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.loginForm =
      this.fb.group({
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
      });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const request =
      this.loginForm.value as LoginRequest;

    this.authService
      .login(request)
      .subscribe({
        next: () => {
          this.isLoading = false;

          this.router.navigate([
            '/dashboard',
          ]);
        },

        error: (error) => {
          this.isLoading = false;

          this.errorMessage =
            error?.error?.message ??
            'Login failed. Please try again.';
        },
      });
  }
}