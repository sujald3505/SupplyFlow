import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  CreateSalesReturnRequest,
  SalesReturn
} from '../models/sales-return.model';

@Injectable({
  providedIn: 'root'
})
export class SalesReturnService {

  private readonly apiUrl =
    'https://localhost:7198/api/SalesReturns';

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(): Observable<SalesReturn[]> {
    return this.http.get<SalesReturn[]>(
      this.apiUrl
    );
  }

  getById(
    id: number
  ): Observable<SalesReturn> {
    return this.http.get<SalesReturn>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: CreateSalesReturnRequest
  ): Observable<SalesReturn> {

    return this.http.post<SalesReturn>(
      this.apiUrl,
      request
    );
  }
}