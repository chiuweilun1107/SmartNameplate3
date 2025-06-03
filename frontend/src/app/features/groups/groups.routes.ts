import { Routes } from '@angular/router';
import { GroupListComponent } from './group-list.component';
import { GroupFormComponent } from './group-form.component';

export const GROUPS_ROUTES: Routes = [
  {
    path: '',
    component: GroupListComponent,
    title: '群組設置'
  },
  {
    path: 'create',
    component: GroupFormComponent,
    title: '新增群組'
  },
  {
    path: 'edit/:id',
    component: GroupFormComponent,
    title: '編輯群組'
  }
];
