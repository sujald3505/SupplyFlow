import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Company,
  UpdateCompanyRequest,
} from '../models/company.model';

@Injectable({
  providedIn: 'root',
})
export class CompanyService {

  private readonly apiUrl =
    'https://localhost:7198/api/Company';

  constructor(
    private readonly http: HttpClient
  ) {}

  getCurrentCompany(): Observable<Company> {
    return this.http.get<Company>(
      this.apiUrl
    );
  }

  updateCompany(
    request: UpdateCompanyRequest
  ): Observable<Company> {
    return this.http.put<Company>(
      this.apiUrl,
      request
    );
  }
}