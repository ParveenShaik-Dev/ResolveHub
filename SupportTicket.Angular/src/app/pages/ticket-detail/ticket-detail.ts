import { Component, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ticket-detail.html'
})
export class TicketDetail {
  ticket: any = null;
  comments: any[] = [];
  newComment = '';
  isEditing = false;
  editTitle = '';
  editDescription = '';
  editPriority = '';
  message = '';
  private apiBase = environment.apiUrl;;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {
    const id = this.route.snapshot.paramMap.get('id');
    this.loadTicket(id);
  }

  headers() {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  loadTicket(id: any) {
    this.http.get<any>(`${this.apiBase}/tickets/${id}`, { headers: this.headers() }).subscribe({
      next: (data) => {
        this.ticket = data;
        this.comments = data.comments || [];
        this.editTitle = data.title;
        this.editDescription = data.description;
        this.editPriority = data.priority;
        this.cdr.detectChanges();
      }
    });
  }

  startEdit() { this.isEditing = true; }

  saveEdit() {
    this.http.put(`${this.apiBase}/tickets/${this.ticket.id}`, {
      title: this.editTitle,
      description: this.editDescription,
      priority: this.editPriority
    }, { headers: this.headers() }).subscribe({
      next: () => {
        this.ticket.title = this.editTitle;
        this.ticket.description = this.editDescription;
        this.ticket.priority = this.editPriority;
        this.isEditing = false;
        this.message = 'Ticket updated!';
        this.cdr.detectChanges();
      }
    });
  }

  deleteTicket() {
    if (!confirm('Delete this ticket?')) return;
    this.http.delete(`${this.apiBase}/tickets/${this.ticket.id}`, { headers: this.headers() }).subscribe({
      next: () => this.router.navigate(['/dashboard'])
    });
  }

  addComment() {
    if (!this.newComment.trim()) return;
    this.http.post<any>(`${this.apiBase}/tickets/${this.ticket.id}/comments`, {
      content: this.newComment
    }, { headers: this.headers() }).subscribe({
      next: (c) => {
        this.comments = [...this.comments, c];
        this.newComment = '';
        this.message = 'Comment added!';
        this.cdr.detectChanges();
      }
    });
  }

  deleteComment(commentId: number) {
    this.http.delete(`${this.apiBase}/tickets/${this.ticket.id}/comments/${commentId}`,
      { headers: this.headers() }).subscribe({
      next: () => {
        this.comments = this.comments.filter(c => c.id !== commentId);
        this.cdr.detectChanges();
      }
    });
  }

  goBack() { this.router.navigate(['/dashboard']); }
}