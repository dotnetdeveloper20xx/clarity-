import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { MatterDto, PaginatedList } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class MatterApiService {
  private baseUrl = `${environment.apiUrl}/matters`;

  constructor(private http: HttpClient) {}

  getMatters(params: { searchTerm?: string; status?: number; clientId?: string; pageNumber?: number; pageSize?: number }): Observable<PaginatedList<MatterDto>> {
    let httpParams = new HttpParams();
    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.status !== undefined) httpParams = httpParams.set('status', params.status.toString());
    if (params.clientId) httpParams = httpParams.set('clientId', params.clientId);
    if (params.pageNumber) httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    return this.http.get<PaginatedList<MatterDto>>(this.baseUrl, { params: httpParams });
  }

  getMatter(id: string): Observable<MatterDto> {
    return this.http.get<MatterDto>(`${this.baseUrl}/${id}`);
  }

  createMatter(matter: any): Observable<string> {
    return this.http.post<string>(this.baseUrl, matter);
  }

  updateStatus(id: string, status: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/status`, { id, newStatus: status });
  }
}
