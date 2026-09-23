import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpParams,
} from '@angular/common/http';

import { Observable } from 'rxjs';

import {
  InventoryStockReportQuery,
  InventoryStockReportResult,

  InventoryTransactionReportQuery,
  InventoryTransactionReportResult,

  LowStockReportQuery,
  LowStockReportResult,

  PurchaseOrderReportQuery,
  PurchaseOrderReportResult,
} from '../models/report.model';


@Injectable({
  providedIn: 'root',
})
export class ReportService {

  private readonly apiUrl =
    'https://localhost:7198/api/Reports';


  constructor(
    private readonly http: HttpClient
  ) {}


  // ========================================
  // INVENTORY STOCK REPORT
  // GET: /api/Reports/inventory-stock
  // ========================================

  getInventoryStockReport(
    query: InventoryStockReportQuery
  ): Observable<InventoryStockReportResult> {

    const params =
      this.buildParams(query);

    return this.http.get<InventoryStockReportResult>(
      `${this.apiUrl}/inventory-stock`,
      { params }
    );
  }


  // ========================================
  // INVENTORY TRANSACTION REPORT
  // GET: /api/Reports/inventory-transactions
  // ========================================

  getInventoryTransactionReport(
    query: InventoryTransactionReportQuery
  ): Observable<InventoryTransactionReportResult> {

    const params =
      this.buildParams(query);

    return this.http.get<InventoryTransactionReportResult>(
      `${this.apiUrl}/inventory-transactions`,
      { params }
    );
  }


  // ========================================
  // LOW STOCK REPORT
  // GET: /api/Reports/low-stock
  // ========================================

  getLowStockReport(
    query: LowStockReportQuery
  ): Observable<LowStockReportResult> {

    const params =
      this.buildParams(query);

    return this.http.get<LowStockReportResult>(
      `${this.apiUrl}/low-stock`,
      { params }
    );
  }


  // ========================================
  // PURCHASE ORDER REPORT
  // GET: /api/Reports/purchase-orders
  // ========================================

  getPurchaseOrderReport(
    query: PurchaseOrderReportQuery
  ): Observable<PurchaseOrderReportResult> {

    const params =
      this.buildParams(query);

    return this.http.get<PurchaseOrderReportResult>(
      `${this.apiUrl}/purchase-orders`,
      { params }
    );
  }


  // ========================================
  // BUILD QUERY PARAMETERS
  // ========================================

  private buildParams(
    query: object
  ): HttpParams {

    let params = new HttpParams();

    Object.entries(query).forEach(
      ([key, value]) => {

        if (
          value !== undefined &&
          value !== null &&
          value !== ''
        ) {

          params = params.set(
            key,
            String(value)
          );

        }

      }
    );

    return params;
  }
}