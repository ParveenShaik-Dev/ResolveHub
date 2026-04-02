import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TicketService } from '../../services/ticket';

@Component({
  selector: 'app-create-ticket',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './create-ticket.html'
})
export class CreateTicket {
  title = ''; description = ''; priority = 'Low'; error = '';
  isLoading =false;
  constructor(private ticketService: TicketService, private router: Router) {}

  submit() {
    this.isLoading =true;
    this.ticketService.create({ title: this.title, description: this.description, priority: this.priority }).subscribe({
      next: () => {
        this.isLoading =false;
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.isLoading =false;
        this.error = 'Failed to create ticket';
      }
      });
  }
}
