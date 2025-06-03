import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  {
    path: 'database-schema',
    loadComponent: () => import('./database-schema.component').then(m => m.DatabaseSchemaComponent)
  }
];
