import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import {
  Warehouse,
  CreateWarehouseRequest,
  UpdateWarehouseRequest,
} from '../models/warehouse.model';

@Injectable({
  providedIn: 'root',
})
export class WarehouseService {

  constructor(
    private readonly api: ApiService
  ) {}

  getAll(): Observable<Warehouse[]> {
    return this.api.get<Warehouse[]>(
      'Warehouses'
    );
  }

  getById(
    id: number
  ): Observable<Warehouse> {
    return this.api.get<Warehouse>(
      `Warehouses/${id}`
    );
  }

  create(
    request: CreateWarehouseRequest
  ): Observable<Warehouse> {
    return this.api.post<Warehouse>(
      'Warehouses',
      request
    );
  }

  update(
    id: number,
    request: UpdateWarehouseRequest
  ): Observable<Warehouse> {
    return this.api.put<Warehouse>(
      `Warehouses/${id}`,
      request
    );
  }
}