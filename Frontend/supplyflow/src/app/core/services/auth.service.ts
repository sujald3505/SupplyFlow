import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { ApiService } from './api.service';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
} from '../models/auth.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly tokenKey = 'supplyflow_token';
  private readonly userKey = 'supplyflow_user';

  constructor(
    private readonly api: ApiService
  ) {}

  login(
    request: LoginRequest
  ): Observable<LoginResponse> {
    return this.api
      .post<LoginResponse>(
        'Auth/login',
        request
      )
      .pipe(
        tap((response) => {
          localStorage.setItem(
            this.tokenKey,
            response.token
          );

          localStorage.setItem(
            this.userKey,
            JSON.stringify(response)
          );
        })
      );
  }
  
  register(
  request: RegisterRequest
): Observable<RegisterResponse> {
  return this.api.post<RegisterResponse>(
    'Auth/register',
    request
  );
}
  logout(): void {
    localStorage.removeItem(
      this.tokenKey
    );

    localStorage.removeItem(
      this.userKey
    );
  }

  getToken(): string | null {
    return localStorage.getItem(
      this.tokenKey
    );
  }

  getCurrentUser(): LoginResponse | null {
    const user = localStorage.getItem(
      this.userKey
    );

    if (!user) {
      return null;
    }

    return JSON.parse(user) as LoginResponse;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}