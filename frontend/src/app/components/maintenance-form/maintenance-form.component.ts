import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';

import { MaintenanceRequestService } from '../../services/maintenance-request.service';
import { 
  MaintenanceRequest, 
  CreateMaintenanceRequest, 
  UpdateMaintenanceRequest, 
  MaintenanceStatus, 
  UserRole 
} from '../../models/maintenance-request.model';

@Component({
  selector: 'app-maintenance-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './maintenance-form.component.html',
  styleUrls: ['./maintenance-form.component.css']
})
export class MaintenanceFormComponent implements OnInit, OnDestroy {
  @Input() requestId: number | null = null;
  @Input() currentRole: UserRole = UserRole.PropertyManager;
  @Output() formClosed = new EventEmitter<void>();
  @Output() formSubmitted = new EventEmitter<void>();

  maintenanceForm: FormGroup;
  loading = false;
  isEditMode = false;
  selectedFile: File | null = null;
  imagePreview: string | null = null;
  existingRequest: MaintenanceRequest | null = null;
  message = '';
  messageType: 'success' | 'error' | '' = '';

  private destroy$ = new Subject<void>();

  // Expose enums to template
  MaintenanceStatus = MaintenanceStatus;
  UserRole = UserRole;

  constructor(
    private fb: FormBuilder,
    private maintenanceService: MaintenanceRequestService
  ) {
    this.maintenanceForm = this.createForm();
  }

  ngOnInit(): void {
    this.isEditMode = this.requestId !== null;
    
    if (this.isEditMode && this.requestId) {
      this.loadMaintenanceRequest(this.requestId);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      maintenanceEventName: ['', [Validators.required, Validators.maxLength(200)]],
      propertyName: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      status: [{ value: MaintenanceStatus.New, disabled: true }]
    });
  }

  private loadMaintenanceRequest(id: number): void {
    this.loading = true;
    this.maintenanceService.getMaintenanceRequestById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (request) => {
          this.existingRequest = request;
          this.populateForm(request);
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading maintenance request:', error);
          this.showMessage('Error loading maintenance request', 'error');
          this.loading = false;
        }
      });
  }

  private populateForm(request: MaintenanceRequest): void {
    this.maintenanceForm.patchValue({
      maintenanceEventName: request.maintenanceEventName,
      propertyName: request.propertyName,
      description: request.description,
      status: request.status
    });

    // Enable status field for admin users
    if (this.isAdmin()) {
      this.maintenanceForm.get('status')?.enable();
    }

    // Load existing image if available
    if (request.imageData) {
      this.imagePreview = `data:image/jpeg;base64,${request.imageData}`;
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      
      // Create image preview
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreview = e.target.result;
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(): void {
    this.selectedFile = null;
    this.imagePreview = null;
  }

  onSubmit(): void {
    if (this.maintenanceForm.valid && !this.loading) {
      this.loading = true;

      if (this.isEditMode && this.requestId) {
        this.updateMaintenanceRequest();
      } else {
        this.createMaintenanceRequest();
      }
    }
  }

  private createMaintenanceRequest(): void {
    const formValue = this.maintenanceForm.value;
    
    this.processImageAndSubmit(formValue, (imageData, fileName) => {
      const createRequest: CreateMaintenanceRequest = {
        maintenanceEventName: formValue.maintenanceEventName,
        propertyName: formValue.propertyName,
        description: formValue.description,
        imageData: imageData,
        imageFileName: fileName,
        createdBy: 'current-user@example.com' // In real app, get from auth service
      };

      this.maintenanceService.createMaintenanceRequest(createRequest)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.showMessage('Maintenance request created successfully', 'success');
            this.loading = false;
            setTimeout(() => this.formSubmitted.emit(), 1500);
          },
          error: (error) => {
            console.error('Error creating maintenance request:', error);
            this.showMessage('Error creating maintenance request', 'error');
            this.loading = false;
          }
        });
    });
  }

  private updateMaintenanceRequest(): void {
    const formValue = this.maintenanceForm.value;
    
    this.processImageAndSubmit(formValue, (imageData, fileName) => {
      const updateRequest: UpdateMaintenanceRequest = {
        maintenanceEventName: formValue.maintenanceEventName,
        propertyName: formValue.propertyName,
        description: formValue.description,
        status: this.isAdmin() ? formValue.status : undefined,
        imageData: imageData,
        imageFileName: fileName,
        updatedBy: 'current-user@example.com' // In real app, get from auth service
      };

      this.maintenanceService.updateMaintenanceRequest(this.requestId!, updateRequest, this.currentRole)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.showMessage('Maintenance request updated successfully', 'success');
            this.loading = false;
            setTimeout(() => this.formSubmitted.emit(), 1500);
          },
          error: (error) => {
            console.error('Error updating maintenance request:', error);
            this.showMessage('Error updating maintenance request', 'error');
            this.loading = false;
          }
        });
    });
  }

  private processImageAndSubmit(formValue: any, callback: (imageData: string | undefined, fileName: string | undefined) => void): void {
    if (this.selectedFile) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const base64String = e.target.result.split(',')[1]; // Remove data:image/jpeg;base64, prefix
        callback(base64String, this.selectedFile!.name);
      };
      reader.readAsDataURL(this.selectedFile);
    } else {
      // Keep existing image data if no new file selected
      const existingImageData = this.existingRequest?.imageData;
      const existingFileName = this.existingRequest?.imageFileName;
      callback(existingImageData, existingFileName);
    }
  }

  private showMessage(text: string, type: 'success' | 'error'): void {
    this.message = text;
    this.messageType = type;
    setTimeout(() => {
      this.message = '';
      this.messageType = '';
    }, 3000);
  }

  closeForm(): void {
    this.formClosed.emit();
  }

  isAdmin(): boolean {
    return this.currentRole === UserRole.Admin;
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

  getFormTitle(): string {
    return this.isEditMode ? 'Edit Maintenance Request' : 'Add New Maintenance Request';
  }

  getSubmitButtonText(): string {
    return this.isEditMode ? 'Update Request' : 'Create Request';
  }

  hasError(fieldName: string): boolean {
    const field = this.maintenanceForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getError(fieldName: string): string {
    const field = this.maintenanceForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) return `${fieldName} is required`;
      if (field.errors['maxlength']) return `Maximum ${field.errors['maxlength'].requiredLength} characters allowed`;
    }
    return '';
  }
}
