import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container">
      <div class="header">
        <h2>My Tickets</h2>
        <div style="display:flex; gap:10px;">
          <button (click)="goCreate()">+ New Ticket</button>
          <button (click)="logout()">Logout</button>
        </div>
      </div>

      <!-- PAGE LOADER -->
      <div *ngIf="isLoading" style="text-align:center; padding:40px;">
        <span class="spinner" style="border-color:#66666655; border-top-color:#667eea; width:30px; height:30px; border-width:3px;"></span>
        <p style="margin-top:10px; color:#888;">Loading tickets...</p>
      </div>

      <!-- TICKETS TABLE -->
      <table *ngIf="!isLoading && tickets.length > 0">
        <thead>
          <tr><th>Title</th><th>Status</th><th>Priority</th></tr>
        </thead>
        <tbody>
          <tr *ngFor="let t of tickets" 
              (click)="router.navigate(['/ticket', t.id])" 
              style="cursor:pointer;">
            <td>{{ t.title }}</td>
            <td>{{ t.status }}</td>
            <td>{{ t.priority }}</td>
          </tr>
        </tbody>
      </table>

      <p *ngIf="!isLoading && tickets.length === 0">{{ message }}</p>
    </div>
  `
})
export class Dashboard {
  tickets: any[] = [];
  message = '';
  isLoading = true;  // ← starts as true

  constructor(
    private http: HttpClient,
    public router: Router,
    private cdr: ChangeDetectorRef
  ) {
    const token = localStorage.getItem('token');
    if (!token) { this.router.navigate(['/login']); return; }

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    this.http.get<any[]>('https://localhost:7005/api/tickets', { headers }).subscribe({
      next: (data) => {
        this.isLoading = false;  // ← stop loader
        this.tickets = [...data];
        this.message = data.length === 0 ? 'No tickets yet.' : '';
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;  // ← stop loader on error too
        this.message = 'Failed to load tickets.';
        this.cdr.detectChanges();
      }
    });
  }

  goCreate() { this.router.navigate(['/create-ticket']); }
  logout() { 
    localStorage.removeItem('token'); 
    localStorage.removeItem('role');
    this.router.navigate(['/login']); 
  }
}