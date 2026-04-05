import { Component, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-users.html'
})
export class AdminUsers {
  users: any[] = [];
  message = '';
  private api = environment.apiUrl;

  constructor(
    private http: HttpClient,
    public router: Router,
    private cdr: ChangeDetectorRef
  ) {
    if (localStorage.getItem('role') !== 'Admin') {
      this.router.navigate(['/login']);
      return;
    }
    this.loadUsers();
  }

  headers() {
    return new HttpHeaders({ Authorization: `Bearer ${localStorage.getItem('token')}` });
  }

  loadUsers() {
    this.http.get<any[]>(`${this.api}/admin/users`, { headers: this.headers() }).subscribe({
      next: (data) => { this.users = [...data]; this.cdr.detectChanges(); }
    });
  }

  promote(id: number, username: string) {
    if (!confirm(`Promote ${username} to Admin?`)) return;
    this.http.put(`${this.api}/admin/users/${id}/promote`, {}, { headers: this.headers() }).subscribe({
      next: () => {
        this.message = `${username} promoted to Admin!`;
        this.loadUsers();
        this.cdr.detectChanges();
      }
    });
  }

  deleteUser(id: number, username: string) {
    if (!confirm(`Delete agent ${username}?`)) return;
    this.http.delete(`${this.api}/admin/agents/${id}`, { headers: this.headers() }).subscribe({
      next: () => {
        this.message = `${username} deleted!`;
        this.users = this.users.filter(u => u.id !== id);
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
