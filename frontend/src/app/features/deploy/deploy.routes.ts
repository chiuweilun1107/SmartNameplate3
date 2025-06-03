import { Routes } from '@angular/router';

export const DEPLOY_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./components/deploy.component').then(m => m.DeployComponent)
  }
]; 