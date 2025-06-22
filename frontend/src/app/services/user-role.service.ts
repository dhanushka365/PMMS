import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable } from 'rxjs';
import { UserRole } from '../models/maintenance-request.model';

@Injectable({
  providedIn: 'root'
})
export class UserRoleService {
  private currentRoleSubject = new BehaviorSubject<UserRole>(UserRole.PropertyManager);
  public currentRole$ = this.currentRoleSubject.asObservable();

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {
    // Only access localStorage in browser environment
    if (isPlatformBrowser(this.platformId)) {
      const savedRole = localStorage.getItem('userRole');
      if (savedRole !== null) {
        this.currentRoleSubject.next(parseInt(savedRole) as UserRole);
      }
    }
  }

  getCurrentRole(): UserRole {
    return this.currentRoleSubject.value;
  }

  setRole(role: UserRole): void {
    this.currentRoleSubject.next(role);
    
    // Only access localStorage in browser environment
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem('userRole', role.toString());
    }
  }

  isAdmin(): boolean {
    return this.getCurrentRole() === UserRole.Admin;
  }

  isPropertyManager(): boolean {
    return this.getCurrentRole() === UserRole.PropertyManager;
  }

  getRoleName(): string {
    return this.isAdmin() ? 'Admin' : 'Property Manager';
  }
}
