import { Routes } from '@angular/router';

export const CARDS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./card-list.component').then(m => m.CardListComponent)
  },
  {
    path: 'new',
    loadComponent: () => import('./card-designer.component').then(m => m.CardDesignerComponent)
  },
  {
    path: 'edit/:id',
    loadComponent: () => import('./card-designer.component').then(m => m.CardDesignerComponent)
  }
  // 其他路由將在後續添加
];