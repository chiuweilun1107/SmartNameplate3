import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/home',
    pathMatch: 'full'
  },
  {
    path: 'home',
    loadComponent: () => import('./features/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'contact',
    loadComponent: () => import('./features/contact/contact.component').then(m => m.ContactComponent)
  },
  {
    path: 'cards',
    loadChildren: () => import('./features/cards/cards.routes').then(m => m.CARDS_ROUTES)
  },
  {
    path: 'groups',
    loadChildren: () => import('./features/groups/groups.routes').then(m => m.GROUPS_ROUTES)
  },
  {
    path: 'deploy',
    loadChildren: () => import('./features/deploy/deploy.routes').then(m => m.DEPLOY_ROUTES)
  },
  {
    path: 'account',
    loadChildren: () => import('./features/account/account.routes').then(m => m.ACCOUNT_ROUTES)
  },
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.routes').then(m => m.adminRoutes)
  },
  {
    path: '**',
    redirectTo: '/home'
  }
];