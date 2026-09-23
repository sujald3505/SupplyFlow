import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Role,
  CreateRoleRequest,
} from '../models/role.model';

@Injectable({
  providedIn: 'root',
})
export class RoleService {

  private readonly apiUrl =
    'https://localhost:7198/api/Roles';

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(): Observable<Role[]> {
    return this.http.get<Role[]>(
      this.apiUrl
    );
  }

  create(
    request: CreateRoleRequest
  ): Observable<Role> {
    return this.http.post<Role>(
      this.apiUrl,
      request
    );
  }
}