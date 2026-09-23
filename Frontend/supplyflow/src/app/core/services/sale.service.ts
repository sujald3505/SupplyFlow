import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import {
  Sale,
  CreateSaleRequest,
} from '../models/sale.model';

@Injectable({
  providedIn: 'root',
})
export class SaleService {

  constructor(
    private readonly api: ApiService
  ) {}

  // =========================
  // GET ALL SALES
  // =========================

  getAll(): Observable<Sale[]> {

    return this.api.get<Sale[]>(
      'Sales'
    );

  }


  // =========================
  // GET SALE BY ID
  // =========================

  getById(
    id: number
  ): Observable<Sale> {

    return this.api.get<Sale>(
      `Sales/${id}`
    );

  }


  // =========================
  // CREATE SALE
  // =========================

  create(
    request: CreateSaleRequest
  ): Observable<Sale> {

    return this.api.post<Sale>(
      'Sales',
      request
    );

  }

}