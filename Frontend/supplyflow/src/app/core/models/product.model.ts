export interface Product {
  id: number;

  categoryId: number;
  categoryName: string;

  unitId: number;
  unitName: string;
  unitSymbol: string;

  name: string;
  sku: string;

  description?: string;

  costPrice: number;
  sellingPrice: number;

  minimumStockLevel: number;
  imageUrl?: string;

  isActive: boolean;
}


export interface CreateProductRequest {
  categoryId: number;
  unitId: number;

  name: string;
  sku: string;

  description?: string;

  costPrice: number;
  sellingPrice: number;

  minimumStockLevel: number;
}


export interface UpdateProductRequest {
  categoryId: number;
  unitId: number;

  name: string;
  sku: string;

  description?: string;

  costPrice: number;
  sellingPrice: number;

  minimumStockLevel: number;

  isActive: boolean;
}