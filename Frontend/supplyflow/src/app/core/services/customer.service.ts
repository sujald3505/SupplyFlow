import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Customer,
  CreateCustomerRequest,
} from '../models/customer.model';

@Injectable({
  providedIn: 'root',
})
export class CustomerService {

  private readonly apiUrl =
    'https://localhost:7198/api/Customers';

  constructor(
    private readonly http: HttpClient
  ) {}

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(
      this.apiUrl
    );
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(
      `${this.apiUrl}/${id}`
    );
  }

  create(
    request: CreateCustomerRequest
  ): Observable<Customer> {
    return this.http.post<Customer>(
      this.apiUrl,
      request
    );
  }
}