import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AuthService } from './auth';

@Injectable({ providedIn: 'root' })
export class TicketService {
  private base = environment.apiUrl;

  constructor(private http: HttpClient, private auth: AuthService) {}

  private headers() {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  getAll() {
    return this.http.get<any[]>(`${this.base}/tickets`, { headers: this.headers() });
  }

  create(data: any) {
    return this.http.post(`${this.base}/tickets`, data, { headers: this.headers() });
  }
}