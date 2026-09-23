import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from './api.service';

import {
  Product,
  CreateProductRequest,
  UpdateProductRequest,
} from '../models/product.model';


@Injectable({
  providedIn: 'root',
})
export class ProductService {

  constructor(
    private readonly api: ApiService
  ) {}


  getAll(): Observable<Product[]> {

    return this.api.get<Product[]>(
      'Products'
    );

  }


  getById(
    id: number
  ): Observable<Product> {

    return this.api.get<Product>(
      `Products/${id}`
    );

  }


  create(
    request: CreateProductRequest
  ): Observable<Product> {

    return this.api.post<Product>(
      'Products',
      request
    );

  }


  update(
    id: number,
    request: UpdateProductRequest
  ): Observable<Product> {

    return this.api.put<Product>(
      `Products/${id}`,
      request
    );

  }

  uploadImage(
  productId: number,
  file: File
): Observable<{ imageUrl: string }> {

  const formData = new FormData();

  formData.append('file', file);

  return this.api.post<{ imageUrl: string }>(
    `Products/${productId}/image`,
    formData
  );
}

}