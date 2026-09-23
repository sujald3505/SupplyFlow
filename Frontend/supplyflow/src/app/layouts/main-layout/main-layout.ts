import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet,
} from '@angular/router';

import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  imports: [
    CommonModule,
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
  ],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.css',
})
export class MainLayout {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly currentUser =
    this.authService.getCurrentUser();

  // Mobile sidebar state
  isSidebarOpen = false;

  toggleSidebar(): void {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  closeSidebar(): void {
    this.isSidebarOpen = false;
  }

  // ==============================
  // ROLE CHECKS
  // ==============================

  isAdmin(): boolean {
    return (
      this.currentUser?.role?.toLowerCase() ===
      'admin'
    );
  }

  isPurchaseManager(): boolean {
    return (
      this.currentUser?.role?.toLowerCase() ===
      'purchasemanager'
    );
  }

  isWarehouseManager(): boolean {
    return (
      this.currentUser?.role?.toLowerCase() ===
      'warehousemanager'
    );
  }

  isEmployee(): boolean {
    return (
      this.currentUser?.role?.toLowerCase() ===
      'employee'
    );
  }

  canSeeReports(): boolean {
    return (
      this.isAdmin() ||
      this.isPurchaseManager() ||
      this.isWarehouseManager()
    );
  }

  logout(): void {
    this.authService.logout();

    this.router.navigate([
      '/login',
    ]);
  }
}