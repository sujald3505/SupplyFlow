import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import {
  Unit,
  CreateUnitRequest,
  UpdateUnitRequest,
} from '../models/unit.model';

@Injectable({
  providedIn: 'root',
})
export class UnitService {

  constructor(
    private readonly api: ApiService
  ) {}

  getAll(): Observable<Unit[]> {
    return this.api.get<Unit[]>(
      'Units'
    );
  }

  getById(id: number): Observable<Unit> {
    return this.api.get<Unit>(
      `Units/${id}`
    );
  }

  create(
    request: CreateUnitRequest
  ): Observable<Unit> {
    return this.api.post<Unit>(
      'Units',
      request
    );
  }

  update(
    id: number,
    request: UpdateUnitRequest
  ): Observable<Unit> {
    return this.api.put<Unit>(
      `Units/${id}`,
      request
    );
  }
}