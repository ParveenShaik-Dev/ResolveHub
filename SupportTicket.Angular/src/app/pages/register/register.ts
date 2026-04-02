import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './register.html'
})
export class Register {
  username = ''; email = ''; password = ''; error = '';
  isLoading =false;
  constructor(private auth: AuthService, private router: Router) {}

  register() {
    this.isLoading =true;
    this.auth.register({ username: this.username, email: this.email, password: this.password }).subscribe({
      next: () => {
        this.isLoading =false;
        this.router.navigate(['/login']);
      },
      error: () => {
        this.isLoading =false;
        this.error = 'Registration failed';
      }
    });
  }
}