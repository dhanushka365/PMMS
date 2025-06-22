import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { MaintenanceRequestService } from '../../services/maintenance-request.service';
import { UserRoleService } from '../../services/user-role.service';
import { MaintenanceRequest, MaintenanceStatus, UserRole } from '../../models/maintenance-request.model';
import { MaintenanceFormComponent } from '../maintenance-form/maintenance-form.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MaintenanceFormComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  maintenanceRequests: MaintenanceRequest[] = [];
  filteredRequests: MaintenanceRequest[] = [];
  loading = false;
  searchTerm = '';
  selectedStatus: MaintenanceStatus | null = null;
  showForm = false;
  selectedRequestId: number | null = null;
  currentRole: UserRole = UserRole.PropertyManager;
  
  private destroy$ = new Subject<void>();

  // Expose enums to template
  MaintenanceStatus = MaintenanceStatus;
  UserRole = UserRole;

  constructor(
    private maintenanceService: MaintenanceRequestService,
    private userRoleService: UserRoleService
  ) {}

  ngOnInit(): void {
    this.userRoleService.currentRole$
      .pipe(takeUntil(this.destroy$))
      .subscribe(role => {
        this.currentRole = role;
      });

    this.loadMaintenanceRequests();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadMaintenanceRequests(): void {
    this.loading = true;
    this.maintenanceService.getAllMaintenanceRequests(this.searchTerm, this.selectedStatus || undefined)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (requests) => {
          this.maintenanceRequests = requests;
          this.filteredRequests = requests;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading maintenance requests:', error);
          this.loading = false;
        }
      });
  }

  onSearch(): void {
    this.loadMaintenanceRequests();
  }

  onStatusFilter(): void {
    this.loadMaintenanceRequests();
  }

  openAddForm(): void {
    this.selectedRequestId = null;
    this.showForm = true;
  }

  openEditForm(request: MaintenanceRequest): void {
    this.selectedRequestId = request.id;
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.selectedRequestId = null;
  }

  onFormSubmitted(): void {
    this.closeForm();
    this.loadMaintenanceRequests();
  }

  getStatusClass(status: MaintenanceStatus): string {
    switch (status) {
      case MaintenanceStatus.New:
        return 'status-new';
      case MaintenanceStatus.Accepted:
        return 'status-accepted';
      case MaintenanceStatus.Rejected:
        return 'status-rejected';
      default:
        return '';
    }
  }

  getStatusText(status: MaintenanceStatus): string {
    switch (status) {
      case MaintenanceStatus.New:
        return 'New';
      case MaintenanceStatus.Accepted:
        return 'Accepted';
      case MaintenanceStatus.Rejected:
        return 'Rejected';
      default:
        return 'Unknown';
    }
  }

  toggleRole(): void {
    const newRole = this.currentRole === UserRole.Admin ? UserRole.PropertyManager : UserRole.Admin;
    this.userRoleService.setRole(newRole);
  }

  isAdmin(): boolean {
    return this.userRoleService.isAdmin();
  }

  getRoleName(): string {
    return this.userRoleService.getRoleName();
  }
}
