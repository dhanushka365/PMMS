import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  MaintenanceRequest, 
  CreateMaintenanceRequest, 
  UpdateMaintenanceRequest, 
  MaintenanceStatus,
  UserRole 
} from '../models/maintenance-request.model';

@Injectable({
  providedIn: 'root'
})
export class MaintenanceRequestService {
  private apiUrl = `${environment.apiUrl}/maintenancerequests`;

  constructor(private http: HttpClient) { }

  getAllMaintenanceRequests(searchTerm?: string, status?: MaintenanceStatus): Observable<MaintenanceRequest[]> {
    let params = new HttpParams();
    
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    
    if (status !== undefined && status !== null) {
      params = params.set('status', status.toString());
    }

    return this.http.get<MaintenanceRequest[]>(this.apiUrl, { params });
  }

  getMaintenanceRequestById(id: number): Observable<MaintenanceRequest> {
    return this.http.get<MaintenanceRequest>(`${this.apiUrl}/${id}`);
  }

  createMaintenanceRequest(request: CreateMaintenanceRequest): Observable<MaintenanceRequest> {
    return this.http.post<MaintenanceRequest>(this.apiUrl, request);
  }

  updateMaintenanceRequest(
    id: number, 
    request: UpdateMaintenanceRequest, 
    userRole: UserRole
  ): Observable<MaintenanceRequest> {
    const params = new HttpParams().set('userRole', userRole.toString());
    return this.http.put<MaintenanceRequest>(`${this.apiUrl}/${id}`, request, { params });
  }

  deleteMaintenanceRequest(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
