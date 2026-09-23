import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';
import {
  Category,
  CreateCategoryRequest,
} from '../models/category.model';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  constructor(
    private readonly api: ApiService
  ) {}

  getAll(): Observable<Category[]> {
    return this.api.get<Category[]>(
      'Categories'
    );
  }

  create(
    request: CreateCategoryRequest
  ): Observable<Category> {
    return this.api.post<Category>(
      'Categories',
      request
    );
  }
}