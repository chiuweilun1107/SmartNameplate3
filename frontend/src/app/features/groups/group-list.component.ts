import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';

interface Group {
  id: number;
  name: string;
  description?: string;
  cardCount: number;
  deviceCount: number;
  createdAt: Date;
}

@Component({
  selector: 'sn-group-list',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './group-list.component.html',
  styleUrls: ['./group-list.component.scss']
})
export class GroupListComponent implements OnInit {
  groups: Group[] = [];
  loading = true;

  constructor(private router: Router) {}

  ngOnInit() {
    this.loadGroups();
  }

  loadGroups() {
    this.loading = true;
    // 模擬載入資料
    setTimeout(() => {
      this.groups = [
        {
          id: 1,
          name: '測試群組',
          description: '用於測試功能的群組',
          cardCount: 3,
          deviceCount: 2,
          createdAt: new Date('2024-01-15')
        },
        {
          id: 2,
          name: '辦公室群組',
          description: '辦公室桌牌群組',
          cardCount: 5,
          deviceCount: 8,
          createdAt: new Date('2024-02-01')
        },
        {
          id: 3,
          name: '會議室群組',
          description: '會議室相關的桌牌設定',
          cardCount: 2,
          deviceCount: 4,
          createdAt: new Date('2024-02-10')
        }
      ];
      this.loading = false;
    }, 1000);
  }

  createGroup() {
    console.log('導航到新增群組頁面');
    this.router.navigate(['/groups/create']);
  }

  onGroupClick(group: Group) {
    console.log('點擊群組:', group);
    this.router.navigate(['/groups', group.id]);
  }

  onGroupDelete(group: Group, event: Event) {
    event.stopPropagation();
    console.log('刪除群組:', group);
    if (confirm(`確定要刪除群組「${group.name}」嗎？`)) {
      this.groups = this.groups.filter(g => g.id !== group.id);
    }
  }

  editGroup(group: Group, event: Event) {
    event.stopPropagation();
    console.log('編輯群組:', group);
    this.router.navigate(['/groups/edit', group.id]);
  }

  trackByGroupId(index: number, group: Group): number {
    return group.id;
  }
} 