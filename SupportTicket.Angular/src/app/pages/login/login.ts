import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './login.html'
})
export class Login {
  email = ''; password = ''; error = '';
  isLoading = false;
  constructor(private http: HttpClient, private router: Router) {}

  login() {
    this.isLoading =true;
    this.http.post<any>('https://localhost:7005/api/auth/login', {
      email: this.email,
      password: this.password
    }).subscribe({
      next: (res) => {
      this.isLoading =false;
      localStorage.setItem('token', res.token);

       // decode role from JWT
      const payload = JSON.parse(atob(res.token.split('.')[1]));
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      localStorage.setItem('role', role);
  
    // redirect based on role
     if (role === 'Admin') {
       this.router.navigate(['/admin']);
      }else if (role === 'Agent') {
       this.router.navigate(['/agent-dashboard']);
      }else {
      this.router.navigate(['/dashboard']);
  }
},
      error: (err) => {
        this.isLoading =false;
        console.log('Error:', err);
        this.error = 'Invalid credentials';
      }
    });
  }
}