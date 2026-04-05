import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './agent-dashboard.html'
})
export class AgentDashboard {
  allTickets: any[] = [];
  myTickets: any[] = [];
  activeTab = 'all';
  message = '';
  private api = environment.apiUrl;

  constructor(
    private http: HttpClient,
    public router: Router,
    private cdr: ChangeDetectorRef
  ) {
    if (localStorage.getItem('role') !== 'Agent') {
      this.router.navigate(['/login']);
      return;
    }
    this.loadAll();
    this.loadMyAssigned();
  }

  headers() {
    return new HttpHeaders({ Authorization: `Bearer ${localStorage.getItem('token')}` });
  }

  loadAll() {
    this.http.get<any[]>(`${this.api}/tickets`, { headers: this.headers() }).subscribe({
      next: (data) => { this.allTickets = [...data]; this.cdr.detectChanges(); }
    });
  }

  loadMyAssigned() {
    this.http.get<any[]>(`${this.api}/tickets/my-assigned`, { headers: this.headers() }).subscribe({
      next: (data) => { this.myTickets = [...data]; this.cdr.detectChanges(); }
    });
  }

  updateStatus(ticket: any, status: string) {
    this.http.put(`${this.api}/tickets/${ticket.id}`, {
      title: ticket.title,
      description: ticket.description,
      priority: ticket.priority,
      status: status
    }, { headers: this.headers() }).subscribe({
      next: () => {
        ticket.status = status;
        this.message = `Ticket #${ticket.id} updated to ${status}`;
        this.cdr.detectChanges();
      }
    });
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    this.router.navigate(['/login']);
  }
}