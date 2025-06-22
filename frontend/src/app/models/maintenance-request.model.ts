export enum MaintenanceStatus {
  New = 0,
  Accepted = 1,
  Rejected = 2
}

export enum UserRole {
  PropertyManager = 0,
  Admin = 1
}

export interface MaintenanceRequest {
  id: number;
  maintenanceEventName: string;
  propertyName: string;
  description: string;
  status: MaintenanceStatus;
  imageFileName?: string;
  imageData?: string;
  createdDate: Date;
  updatedDate?: Date;
  createdBy: string;
  updatedBy?: string;
}

export interface CreateMaintenanceRequest {
  maintenanceEventName: string;
  propertyName: string;
  description: string;
  imageFileName?: string;
  imageData?: string;
  createdBy: string;
}

export interface UpdateMaintenanceRequest {
  maintenanceEventName: string;
  propertyName: string;
  description: string;
  status?: MaintenanceStatus;
  imageFileName?: string;
  imageData?: string;
  updatedBy: string;
}
