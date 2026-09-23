import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  GoodsReceipt,
  CreateGoodsReceipt,
} from '../models/goods-receipt.model';

@Injectable({
  providedIn: 'root',
})
export class GoodsReceiptService {

  private readonly apiUrl =
    'https://localhost:7198/api/GoodsReceipts';

  constructor(
    private http: HttpClient
  ) {}


  // GET ALL GOODS RECEIPTS
  getAll(): Observable<GoodsReceipt[]> {
    return this.http.get<GoodsReceipt[]>(
      this.apiUrl
    );
  }


  // GET GOODS RECEIPT BY ID
  getById(
    id: number
  ): Observable<GoodsReceipt> {

    return this.http.get<GoodsReceipt>(
      `${this.apiUrl}/${id}`
    );
  }


  // CREATE GOODS RECEIPT
  create(
    request: CreateGoodsReceipt
  ): Observable<GoodsReceipt> {

    return this.http.post<GoodsReceipt>(
      this.apiUrl,
      request
    );
  }


  // COMPLETE GOODS RECEIPT
  complete(
    id: number
  ): Observable<GoodsReceipt> {

    return this.http.post<GoodsReceipt>(
      `${this.apiUrl}/${id}/complete`,
      {}
    );
  }


  // CANCEL GOODS RECEIPT
  cancel(
    id: number
  ): Observable<GoodsReceipt> {

    return this.http.post<GoodsReceipt>(
      `${this.apiUrl}/${id}/cancel`,
      {}
    );
  }
}