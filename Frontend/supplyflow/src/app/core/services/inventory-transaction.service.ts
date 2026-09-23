import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import {
  InventoryTransaction,
  CreateInventoryTransactionRequest,
} from '../models/inventory-transaction.model';


@Injectable({
  providedIn: 'root',
})
export class InventoryTransactionService {

  constructor(
    private readonly api: ApiService
  ) {}


  getAll(): Observable<InventoryTransaction[]> {

    return this.api.get<
      InventoryTransaction[]
    >(
      'InventoryTransactions'
    );

  }


  getByWarehouse(
    warehouseId: number
  ): Observable<InventoryTransaction[]> {

    return this.api.get<
      InventoryTransaction[]
    >(
      `InventoryTransactions/warehouse/${warehouseId}`
    );

  }


  getByProduct(
    productId: number
  ): Observable<InventoryTransaction[]> {

    return this.api.get<
      InventoryTransaction[]
    >(
      `InventoryTransactions/product/${productId}`
    );

  }


  create(
    request: CreateInventoryTransactionRequest
  ): Observable<InventoryTransaction> {

    return this.api.post<
      InventoryTransaction
    >(
      'InventoryTransactions',
      request
    );

  }

}