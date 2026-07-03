import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { adminGuard } from './core/auth/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/login/login.component').then(m => m.LoginComponent),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
  },
  {
    path: 'siniestros',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/siniestros/siniestros-list/siniestros-list.component').then(
        m => m.SiniestrosListComponent
      ),
  },
  {
    path: 'incidencias',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/incidencias/incidencias-list/incidencias-list.component').then(
        m => m.IncidenciasListComponent
      ),
  },
  {
    path: 'expedientes',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/expedientes/expedientes-list/expedientes-list.component').then(
        m => m.ExpedientesListComponent
      ),
  },
  {
    path: 'usuarios',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/usuarios/usuarios-list.component').then(
        m => m.UsuariosListComponent
      ),
  },
  {
    path: 'backup',
    canActivate: [authGuard, adminGuard],
    loadComponent: () =>
      import('./features/backup/backup.component').then(m => m.BackupComponent),
  },
  { path: '**', redirectTo: '/dashboard' },
];
