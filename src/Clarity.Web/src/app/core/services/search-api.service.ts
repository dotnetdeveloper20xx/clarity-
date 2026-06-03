import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';

export interface SearchResults {
  clients: SearchResultItem[];
  matters: SearchResultItem[];
  invoices: SearchResultItem[];
}

export interface SearchResultItem {
  id: string;
  title: string;
  subtitle: string;
  entityType: string;
}

@Injectable({ providedIn: 'root' })
export class SearchApiService {
  constructor(private http: HttpClient) {}

  search(term: string): Observable<SearchResults> {
    return this.http.get<SearchResults>(`${environment.apiUrl}/search`, { params: { q: term } });
  }
}
