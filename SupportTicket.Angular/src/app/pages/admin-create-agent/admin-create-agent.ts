import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-admin-create-agent',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-create-agent.html'
})
export class AdminCreateAgent {
  username = '';
  email = '';
  password = '';
  message = '';
  error = '';
  private api = 'https://localhost:7005/api';

  constructor(private http: HttpClient, public router: Router) {
    // only admin can access
    if (localStorage.getItem('role') !== 'Admin') {
      this.router.navigate(['/login']);
    }
  }

  headers() {
    return new HttpHeaders({ Authorization: `Bearer ${localStorage.getItem('token')}` });
  }
  isLoading =false;
  createAgent() {
    if (!this.username || !this.email || !this.password) {
      this.error = 'All fields required!';
      return;
    }
    this.isLoading =true;
    this.http.post(`${this.api}/admin/agents`, {
      username: this.username,
      email: this.email,
      password: this.password
    }, { headers: this.headers() }).subscribe({
      next: () => {
        this.isLoading =false;
        this.message = `Agent ${this.username} created successfully!`;
        this.username = '';
        this.email = '';
        this.password = '';
        this.error = '';
      },
      error: (err) => {
        this.isLoading =false;
        this.error = err.error?.message || 'Failed to create agent';
      }
    });
  }
}