import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  PurchaseOrder,
  CreatePurchaseOrder,
} from '../models/purchase-order.model';

@Injectable({
  providedIn: 'root',
})
export class PurchaseOrderService {
  private readonly apiUrl =
    'https://localhost:7198/api/PurchaseOrders';

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(): Observable<PurchaseOrder[]> {
    return this.http.get<PurchaseOrder[]>(
      this.apiUrl
    );
  }

  getById(
    id: number
  ): Observable<PurchaseOrder> {
    return this.http.get<PurchaseOrder>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: CreatePurchaseOrder
  ): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      this.apiUrl,
      request
    );
  }

  send(
    id: number
  ): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      `${this.apiUrl}/${id}/send`,
      {}
    );
  }

  confirm(
    id: number
  ): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      `${this.apiUrl}/${id}/confirm`,
      {}
    );
  }

  cancel(
    id: number
  ): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(
      `${this.apiUrl}/${id}/cancel`,
      {}
    );
  }
}