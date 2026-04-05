import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.html'
})
export class Admin {
  tickets: any[] = [];
  agents: any[] = [];
  message = '';
  private api = environment.apiUrl;

  constructor(
    private http: HttpClient,
    public router: Router,
    private cdr: ChangeDetectorRef
  ) {
    if (localStorage.getItem('role') !== 'Admin') {
      this.router.navigate(['/dashboard']);
      return;
    }
    this.loadAll();
    this.loadAgents();
  }

  headers() {
    return new HttpHeaders({ Authorization: `Bearer ${localStorage.getItem('token')}` });
  }

  loadAll() {
    this.http.get<any[]>(`${this.api}/tickets`, { headers: this.headers() }).subscribe({
      next: (data) => { this.tickets = [...data]; this.cdr.detectChanges(); }
    });
  }

  loadAgents() {
    this.http.get<any[]>(`${this.api}/admin/agents`, { headers: this.headers() }).subscribe({
      next: (data) => { this.agents = [...data]; this.cdr.detectChanges(); }
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

  assignTicket(ticketId: number, agentId: string) {
    if (!agentId) return;
    this.http.put(`${this.api}/tickets/${ticketId}/assign`,
      { agentId: parseInt(agentId) },
      { headers: this.headers() }
    ).subscribe({
      next: () => {
        this.message = `Ticket #${ticketId} assigned successfully!`;
        this.cdr.detectChanges();
      }
    });
  }

  deleteTicket(id: number) {
    if (!confirm('Delete this ticket?')) return;
    this.http.delete(`${this.api}/tickets/${id}`, { headers: this.headers() }).subscribe({
      next: () => {
        this.tickets = this.tickets.filter(t => t.id !== id);
        this.message = 'Ticket deleted!';
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