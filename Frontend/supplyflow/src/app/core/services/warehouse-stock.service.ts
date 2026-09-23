import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import { WarehouseStock } from '../models/warehouse-stock.model';


@Injectable({
  providedIn: 'root',
})
export class WarehouseStockService {

  constructor(
    private readonly api: ApiService
  ) {}


  getAll(): Observable<WarehouseStock[]> {

    return this.api.get<WarehouseStock[]>(
      'WarehouseStocks'
    );

  }


  getByWarehouse(
    warehouseId: number
  ): Observable<WarehouseStock[]> {

    return this.api.get<WarehouseStock[]>(
      `WarehouseStocks/warehouse/${warehouseId}`
    );

  }


  getByProduct(
    productId: number
  ): Observable<WarehouseStock[]> {

    return this.api.get<WarehouseStock[]>(
      `WarehouseStocks/product/${productId}`
    );

  }


  getLowStock(): Observable<any[]> {

    return this.api.get<any[]>(
      'WarehouseStocks/low-stock'
    );

  }

}