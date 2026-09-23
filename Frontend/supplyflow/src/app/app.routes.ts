import { Routes } from '@angular/router';

import { Login } from './features/auth/login/login';
import { Dashboard } from './features/dashboard/dashboard';
import { authGuard } from './core/guards/auth.guard';
import { MainLayout } from './layouts/main-layout/main-layout';
import { Categories } from './features/categories/categories';
import { Units } from './features/units/units';
import { Warehouses } from './features/warehouses/warehouses';
import { Suppliers } from './features/suppliers/suppliers';
import { Products } from './features/products/products';
import { WarehouseStocks } from './features/warehouse-stocks/warehouse-stocks';
import { InventoryTransactions } from './features/inventory-transactions/inventory-transactions';
import { PurchaseRequests } from './features/purchase-requests/purchase-requests';
import { PurchaseOrders } from './features/purchase-orders/purchase-orders';
import { GoodsReceipts } from './features/goods-receipts/goods-receipts';
import { CompanyComponent } from './features/company/company';
import { Roles } from './features/roles/roles';
import { Users } from './features/users/users';
import { Reports } from './features/reports/reports';
import { Register } from './features/auth/register/register/register';
import { Sales } from './features/sales/sales';
import { Bill } from './features/bills/bills';
import { SalesReturnComponent } from './features/sales-return/sales-return';

export const routes: Routes = [
  {
    path: 'login',
    component: Login,
  },
  { path: 'register', component: Register },

  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
      },
      {
        path: 'categories',
        component: Categories,
      },
      {
        path: 'units',
        component: Units,
      },
      {
        path: 'warehouses',
        component: Warehouses,
      },
      {
        path: 'suppliers',
        component: Suppliers,
      },
      {
        path: 'products',
        component: Products,
      },
      {
        path: 'warehouse-stocks',
        component: WarehouseStocks,
      },
      {
        path: 'sales',
        component: Sales,
      },
      {
        path: 'sales/:id/bill',
        component: Bill,
      },

      {
        path: 'sales-returns',
        component: SalesReturnComponent,
      },
      {
        path: 'inventory-transactions',
        component: InventoryTransactions,
      },
      {
        path: 'purchase-requests',
        component: PurchaseRequests,
      },

      {
        path: 'purchase-orders',
        component: PurchaseOrders,
      },

      {
        path: 'goods-receipts',
        component: GoodsReceipts,
      },
      {
        path: 'company',
        component: CompanyComponent,
      },
      {
        path: 'roles',
        component: Roles,
      },
      {
        path: 'users',
        component: Users,
      },

      {
        path: 'reports',
        component: Reports,
      },
    ],
  },

  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },

  {
    path: '**',
    redirectTo: 'dashboard',
  },
];
