import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Dashboard } from './pages/dashboard/dashboard';
import { CreateTicket } from './pages/create-ticket/create-ticket';
import { AuthGuard } from './guards/auth-guard';
import { TicketDetail } from './pages/ticket-detail/ticket-detail';
import { Admin } from './pages/admin/admin';
import { AdminUsers } from './pages/admin-users/admin-users';
import { AdminCreateAgent } from './pages/admin-create-agent/admin-create-agent';
import { AgentDashboard } from './pages/agent-dashboard/agent-dashboard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'dashboard', component: Dashboard, canActivate: [AuthGuard] },
  { path: 'create-ticket', component: CreateTicket, canActivate: [AuthGuard] },
  { path: 'ticket/:id', component: TicketDetail, canActivate: [AuthGuard] },
  { path: 'admin', component: Admin, canActivate: [AuthGuard] },
  { path: 'admin/users', component: AdminUsers, canActivate: [AuthGuard] },
  { path: 'admin/create-agent', component: AdminCreateAgent, canActivate: [AuthGuard] },
  { path: 'agent-dashboard', component: AgentDashboard, canActivate: [AuthGuard] },
];
