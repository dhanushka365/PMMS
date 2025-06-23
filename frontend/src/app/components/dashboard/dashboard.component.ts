import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { MaintenanceRequestService } from '../../services/maintenance-request.service';
import { UserRoleService } from '../../services/user-role.service';
import { MaintenanceRequest, MaintenanceStatus, UserRole, UpdateMaintenanceRequest } from '../../models/maintenance-request.model';
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

  quickUpdateStatus(request: MaintenanceRequest, newStatus: MaintenanceStatus, event: Event): void {
    event.stopPropagation(); // Prevent opening the edit form
    
    const updateRequest: UpdateMaintenanceRequest = {
      maintenanceEventName: request.maintenanceEventName,
      propertyName: request.propertyName,
      description: request.description,
      status: newStatus,
      imageFileName: request.imageFileName,
      imageData: request.imageData,
      updatedBy: 'admin@example.com'
    };

    console.log('Quick updating status for request:', request.id, 'to status:', newStatus);
    
    this.maintenanceService.updateMaintenanceRequest(request.id, updateRequest, this.currentRole)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updatedRequest) => {
          console.log('Status updated successfully:', updatedRequest);
          this.loadMaintenanceRequests(); // Refresh the list
        },
        error: (error) => {
          console.error('Error updating status:', error);
          // You could add a toast notification here
        }
      });
  }

  canEditRequest(request: MaintenanceRequest): boolean {
    // Admins can always edit any request
    if (this.isAdmin()) {
      return true;
    }
    
    // Property managers can only edit requests with "New" status
    return request.status === MaintenanceStatus.New;
  }

  openEditFormIfAllowed(request: MaintenanceRequest): void {
    // Always allow opening the form to view details
    // The form itself will handle edit restrictions
    this.openEditForm(request);
  }

  canDeleteRequest(request: MaintenanceRequest): boolean {
    // Only property managers can delete, and only when status is New
    return this.currentRole === UserRole.PropertyManager && 
           request.status === MaintenanceStatus.New;
  }

  confirmDeleteRequest(request: MaintenanceRequest, event: Event): void {
    event.stopPropagation(); // Prevent opening the edit form
    
    if (confirm(`Are you sure you want to delete "${request.maintenanceEventName}"? This action cannot be undone.`)) {
      this.deleteRequest(request.id);
    }
  }

  private deleteRequest(id: number): void {
    this.maintenanceService.deleteMaintenanceRequest(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          console.log('Request deleted successfully');
          this.loadMaintenanceRequests(); // Refresh the list
        },
        error: (error) => {
          console.error('Error deleting request:', error);
          // You could add a toast notification here
        }
      });
  }
}
