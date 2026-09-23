import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import {
  PurchaseRequest,
  CreatePurchaseRequestRequest,
  RejectPurchaseRequestRequest,
} from '../models/purchase-request.model';


@Injectable({
  providedIn: 'root',
})
export class PurchaseRequestService {

  constructor(
    private readonly api: ApiService
  ) {}


  getAll(): Observable<PurchaseRequest[]> {

    return this.api.get<
      PurchaseRequest[]
    >(
      'PurchaseRequests'
    );

  }


  getById(
    id: number
  ): Observable<PurchaseRequest> {

    return this.api.get<
      PurchaseRequest
    >(
      `PurchaseRequests/${id}`
    );

  }


  create(
    request: CreatePurchaseRequestRequest
  ): Observable<PurchaseRequest> {

    return this.api.post<
      PurchaseRequest
    >(
      'PurchaseRequests',
      request
    );

  }


  submit(
    id: number
  ): Observable<PurchaseRequest> {

    return this.api.post<
      PurchaseRequest
    >(
      `PurchaseRequests/${id}/submit`,
      {}
    );

  }


  approve(
    id: number
  ): Observable<PurchaseRequest> {

    return this.api.post<
      PurchaseRequest
    >(
      `PurchaseRequests/${id}/approve`,
      {}
    );

  }


  reject(
    id: number,
    request: RejectPurchaseRequestRequest
  ): Observable<PurchaseRequest> {

    return this.api.post<
      PurchaseRequest
    >(
      `PurchaseRequests/${id}/reject`,
      request
    );

  }

}