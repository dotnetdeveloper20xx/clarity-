import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  {
    path: '',
    loadComponent: () => import('./shell/shell.component').then(m => m.ShellComponent),
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { path: 'clients', loadComponent: () => import('./features/clients/client-list/client-list.component').then(m => m.ClientListComponent) },
      { path: 'clients/:id', loadComponent: () => import('./features/clients/client-detail/client-detail.component').then(m => m.ClientDetailComponent) },
      { path: 'matters', loadComponent: () => import('./features/matters/matter-list/matter-list.component').then(m => m.MatterListComponent) },
      { path: 'matters/:id', loadComponent: () => import('./features/matters/matter-detail/matter-detail.component').then(m => m.MatterDetailComponent) },
      { path: 'time-recording', loadComponent: () => import('./features/time-recording/time-list/time-list.component').then(m => m.TimeListComponent) },
      { path: 'billing', loadComponent: () => import('./features/billing/invoice-list/invoice-list.component').then(m => m.InvoiceListComponent) },
      { path: 'compliance', loadComponent: () => import('./features/compliance/compliance-list/compliance-list.component').then(m => m.ComplianceListComponent) },
      { path: 'notifications', loadComponent: () => import('./features/notifications/notification-list/notification-list.component').then(m => m.NotificationListComponent) },
    ]
  },
  { path: '**', redirectTo: '' }
];
