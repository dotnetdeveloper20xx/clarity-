import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export interface DashboardData {
  openMattersCount: number;
  overdueTasksCount: number;
  pendingComplianceCount: number;
  unreadNotificationsCount: number;
  unbilledTimeValue: number;
  outstandingInvoicesTotal: number;
  paidThisMonthTotal: number;
  complianceAlertsCount: number;
  draftTimeEntriesCount: number;
  pendingApprovalsCount: number;
}

export interface FinancialSummary {
  totalBilledThisMonth: number;
  totalPaidThisMonth: number;
  totalOutstanding: number;
  totalWip: number;
  agedDebt: { band: string; amount: number; invoiceCount: number }[];
  topClients: { clientId: string; clientName: string; revenue: number }[];
}

@Injectable({ providedIn: 'root' })
export class DashboardApiService {
  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardData> {
    return this.http.get<DashboardData>(`${environment.apiUrl}/dashboard`);
  }

  getFinancialSummary(): Observable<FinancialSummary> {
    return this.http.get<FinancialSummary>(`${environment.apiUrl}/dashboard/financial`);
  }
}
