import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'candidatures', pathMatch: 'full' },
  {
    path: 'candidatures',
    loadComponent: () =>
      import('./components/dashboard/dashboard').then((m) => m.Dashboard)
  },
  {
    path: 'candidatures/creer',
    loadComponent: () =>
      import('./components/candidature-create/candidature-create').then(
        (m) => m.CandidatureCreate
      )
  },
  { path: '**', redirectTo: 'candidatures' }
];
