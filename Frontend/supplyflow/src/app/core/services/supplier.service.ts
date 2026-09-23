import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import {
  Supplier,
  CreateSupplierRequest,
  UpdateSupplierRequest,
} from '../models/supplier.model';

@Injectable({
  providedIn: 'root',
})
export class SupplierService {

  constructor(
    private readonly api: ApiService
  ) {}

  getAll(): Observable<Supplier[]> {
    return this.api.get<Supplier[]>(
      'Suppliers'
    );
  }

  getById(
    id: number
  ): Observable<Supplier> {
    return this.api.get<Supplier>(
      `Suppliers/${id}`
    );
  }

  create(
    request: CreateSupplierRequest
  ): Observable<Supplier> {
    return this.api.post<Supplier>(
      'Suppliers',
      request
    );
  }

  update(
    id: number,
    request: UpdateSupplierRequest
  ): Observable<Supplier> {
    return this.api.put<Supplier>(
      `Suppliers/${id}`,
      request
    );
  }
}