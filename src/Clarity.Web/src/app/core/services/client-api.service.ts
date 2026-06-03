import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { ClientDto, PaginatedList } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ClientApiService {
  private baseUrl = `${environment.apiUrl}/clients`;

  constructor(private http: HttpClient) {}

  getClients(params: { searchTerm?: string; status?: number; pageNumber?: number; pageSize?: number }): Observable<PaginatedList<ClientDto>> {
    let httpParams = new HttpParams();
    if (params.searchTerm) httpParams = httpParams.set('searchTerm', params.searchTerm);
    if (params.status !== undefined) httpParams = httpParams.set('status', params.status.toString());
    if (params.pageNumber) httpParams = httpParams.set('pageNumber', params.pageNumber.toString());
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    return this.http.get<PaginatedList<ClientDto>>(this.baseUrl, { params: httpParams });
  }

  getClient(id: string): Observable<ClientDto> {
    return this.http.get<ClientDto>(`${this.baseUrl}/${id}`);
  }

  createClient(client: Partial<ClientDto>): Observable<string> {
    return this.http.post<string>(this.baseUrl, client);
  }

  updateClient(id: string, client: Partial<ClientDto>): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { id, ...client });
  }

  deleteClient(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
