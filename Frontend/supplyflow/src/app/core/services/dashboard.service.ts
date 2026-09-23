import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';
import { DashboardData } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(
    private readonly api: ApiService
  ) {}

  getDashboard(): Observable<DashboardData> {
    return this.api.get<DashboardData>(
      'Dashboard'
    );
  }
}