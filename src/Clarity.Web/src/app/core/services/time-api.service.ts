import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { TimeEntryDto, PaginatedList } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class TimeApiService {
  private baseUrl = `${environment.apiUrl}/timeentries`;

  constructor(private http: HttpClient) {}

  getTimeEntries(params: { matterId?: string; userId?: string; status?: number; fromDate?: string; toDate?: string; pageNumber?: number; pageSize?: number }): Observable<PaginatedList<TimeEntryDto>> {
    let httpParams = new HttpParams();
    if (params.matterId) httpParams = httpParams.set('matterId', params.matterId);
    if (params.userId) httpParams = httpParams.set('userId', params.userId);
    if (params.status !== undefined) httpParams = httpParams.set('status', params.status.toString());
    if (params.fromDate) httpParams = httpParams.set('fromDate', params.fromDate);
    if (params.toDate) httpParams = httpParams.set('toDate', params.toDate);
    if (params.pageNumber) httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    return this.http.get<PaginatedList<TimeEntryDto>>(this.baseUrl, { params: httpParams });
  }

  recordTime(entry: any): Observable<string> {
    return this.http.post<string>(this.baseUrl, entry);
  }

  approveEntry(id: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/approve`, {});
  }
}
