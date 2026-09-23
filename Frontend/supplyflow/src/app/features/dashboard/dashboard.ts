import {
  ChangeDetectorRef,
  Component,
  OnInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardService } from '../../core/services/dashboard.service';
import {
  DashboardData,
} from '../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  dashboard: DashboardData | null = null;

  isLoading = true;
  errorMessage = '';

  constructor(
    private readonly dashboardService: DashboardService,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.dashboardService
      .getDashboard()
      .subscribe({
        next: (response) => {
          this.dashboard = response;
          this.isLoading = false;

          this.cdr.detectChanges();
        },

        error: (error) => {
          this.errorMessage =
            error?.error?.message ??
            'Failed to load dashboard data.';

          this.isLoading = false;

          this.cdr.detectChanges();
        },
      });
  }
}