export interface WarehouseStock {
  id: number;

  warehouseId: number;
  warehouseName: string;

  productId: number;
  productName: string;

  sku: string;

  unitName: string;
  unitSymbol: string;

  quantity: number;
}