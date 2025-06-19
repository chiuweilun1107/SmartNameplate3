import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

interface DatabaseColumn {
  name: string;
  type: string;
  nullable: boolean;
  defaultValue?: string;
}

type DatabaseRow = Record<string, unknown>;

@Component({
  selector: 'sn-database-schema',
  standalone: true,
  imports: [CommonModule, HttpClientModule, FormsModule, MatIconModule],
  template: `
    <div style="display: flex; min-height: unset;">
      <aside class="db-schema__sidebar">
        <h3 class="db-schema__sidebar-title">資料表</h3>
        <div class="db-schema__sidebar-divider"></div>
        <ul class="db-schema__sidebar-list">
          <li *ngFor="let table of tables" 
              (click)="selectTable(table)" 
              (keydown.enter)="selectTable(table)"
              (keydown.space)="selectTable(table)"
              [class.db-schema__sidebar-item--active]="table === selectedTable" 
              class="db-schema__sidebar-item"
              tabindex="0"
              role="button"
              [attr.aria-label]="'選擇資料表 ' + table">
            {{ table }}
          </li>
        </ul>
      </aside>
      <main style="flex: 1; padding: 32px; overflow: auto; background: #e3f0fa;">
        <h2 *ngIf="selectedTable">{{ selectedTable }} <span style="font-size: 14px; color: #888;">欄位</span></h2>
        <div class="db-schema__table-wrapper">
          <div class="db-schema__table-container">
            <table class="db-schema__table" *ngIf="columns.length > 0 && pagedRows.length > 0">
              <thead>
                <tr style="background: #1976d2;">
                  <th *ngFor="let col of columns; let last = last" 
                      [style.borderRight]="!last ? '2px solid #1976d2' : ''" 
                      class="db-schema__table-header">
                    {{ col.name }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let row of pagedRows; let i = index" 
                    [ngStyle]="{'background': i % 2 === 0 ? '#fff' : '#f5f7fa'}" 
                    (mouseenter)="row['_hover'] = true" 
                    (mouseleave)="row['_hover'] = false" 
                    [style.background]="row['_hover'] ? '#d0e6ff' : (i % 2 === 0 ? '#fff' : '#f5f7fa')">
                  <td *ngFor="let col of columns; let last = last" 
                      [style.borderRight]="!last ? '2px solid #888' : ''"
                      class="db-schema__table-cell">
                    <ng-container *ngIf="col.name === 'Thumbnail' || col.name === 'ThumbnailUrl' || col.name === 'LayoutData'; else contentOrNormalCell">
                      <div class="cell-content">
                        <ng-container *ngIf="getRowValue(row, col.name) && getRowValue(row, col.name).length > 17; else shortThumb">
                          <span class="cell-text" 
                                [class.expanded]="getExpandedState(row, col.name)"
                                (click)="toggleExpand(row, col.name)"
                                (keydown.enter)="toggleExpand(row, col.name)"
                                (keydown.space)="toggleExpand(row, col.name)"
                                tabindex="0"
                                role="button"
                                [attr.aria-label]="'展開或收合內容'">
                            {{ getExpandedState(row, col.name) ? getRowValue(row, col.name) : (getRowValue(row, col.name) | slice:0:17) + '...' }}
                          </span>
                          <mat-icon class="expand-icon" 
                                   (click)="toggleExpand(row, col.name)"
                                    (keydown.enter)="toggleExpand(row, col.name)"
                                    (keydown.space)="toggleExpand(row, col.name)"
                                    tabindex="0"
                                    role="button"
                                    [attr.aria-label]="'展開或收合內容'"
                                   [class.rotated]="getExpandedState(row, col.name)">
                            expand_more
                          </mat-icon>
                        </ng-container>
                        <ng-template #shortThumb>{{ getRowValue(row, col.name) || '' }}</ng-template>
                      </div>
                    </ng-container>
                    <ng-template #contentOrNormalCell>
                      <ng-container *ngIf="col.name === 'Content' || col.name === 'ContentA' || col.name === 'ContentB'; else normalCell">
                        <div class="cell-content">
                          <ng-container *ngIf="extractContentText(getRowValue(row, col.name)).length > 17; else shortContent">
                            <span class="cell-text" 
                                  [class.expanded]="getExpandedState(row, col.name)"
                                  (click)="toggleExpand(row, col.name)"
                                  (keydown.enter)="toggleExpand(row, col.name)"
                                  (keydown.space)="toggleExpand(row, col.name)"
                                  tabindex="0"
                                  role="button"
                                  [attr.aria-label]="'展開或收合內容'">
                              {{ getExpandedState(row, col.name) ? extractContentText(getRowValue(row, col.name)) : (extractContentText(getRowValue(row, col.name)) | slice:0:17) + '...' }}
                            </span>
                            <mat-icon class="expand-icon" 
                                     (click)="toggleExpand(row, col.name)"
                                      (keydown.enter)="toggleExpand(row, col.name)"
                                      (keydown.space)="toggleExpand(row, col.name)"
                                      tabindex="0"
                                      role="button"
                                      [attr.aria-label]="'展開或收合內容'"
                                     [class.rotated]="getExpandedState(row, col.name)">
                              expand_more
                            </mat-icon>
                          </ng-container>
                          <ng-template #shortContent>{{ extractContentText(getRowValue(row, col.name)) }}</ng-template>
                        </div>
                      </ng-container>
                      <ng-template #normalCell>
                        <div class="cell-content">
                          <ng-container *ngIf="getRowValue(row, col.name) && getRowValue(row, col.name).toString().length > 17; else shortNormal">
                            <span class="cell-text" 
                                  [class.expanded]="getExpandedState(row, col.name)"
                                  (click)="toggleExpand(row, col.name)"
                                  (keydown.enter)="toggleExpand(row, col.name)"
                                  (keydown.space)="toggleExpand(row, col.name)"
                                  tabindex="0"
                                  role="button"
                                  [attr.aria-label]="'展開或收合內容'">
                              {{ getExpandedState(row, col.name) ? getRowValue(row, col.name) : (getRowValue(row, col.name).toString() | slice:0:17) + '...' }}
                            </span>
                            <mat-icon class="expand-icon" 
                                     (click)="toggleExpand(row, col.name)"
                                      (keydown.enter)="toggleExpand(row, col.name)"
                                      (keydown.space)="toggleExpand(row, col.name)"
                                      tabindex="0"
                                      role="button"
                                      [attr.aria-label]="'展開或收合內容'"
                                     [class.rotated]="getExpandedState(row, col.name)">
                              expand_more
                            </mat-icon>
                          </ng-container>
                          <ng-template #shortNormal>{{ getRowValue(row, col.name) }}</ng-template>
                        </div>
                      </ng-template>
                    </ng-template>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div *ngIf="loadingRows" style="color: #1976d2;">資料載入中...</div>
          <div *ngIf="!loadingRows && pagedRows.length === 0 && columns.length > 0" style="color: #888;">（無資料）</div>
        </div>
        <div style="margin-top: 16px; display: flex; align-items: center; gap: 16px;">
          <button (click)="prevPage()" [disabled]="pageIndex === 0">上一頁</button>
          <span>第 {{ pageIndex + 1 }} / {{ totalPages }} 頁</span>
          <button (click)="nextPage()" [disabled]="pageIndex >= totalPages - 1">下一頁</button>
          <span>每頁顯示
            <select [(ngModel)]="pageSize" (change)="onPageSizeChange()">
              <option *ngFor="let size of pageSizeOptions" [value]="size">{{ size }}</option>
            </select> 筆
          </span>
          <span>共 {{ rows.length }} 筆</span>
        </div>
      </main>
    </div>
  `,
  styles: [
    `.db-schema__table-wrapper { 
       width: 100%; 
       background: #e3f0fa; 
       border-radius: 12px; 
       padding: 24px; 
       box-shadow: 0 2px 8px rgba(0,0,0,0.1);
     }
     .db-schema__table-container {
       width: 100%;
       overflow-x: auto;
       overflow-y: visible;
       border-radius: 8px;
       border: 1px solid #ccc;
       background: white;
       /* 強制顯示滾動條 */
       scrollbar-width: auto; /* Firefox */
       -webkit-overflow-scrolling: touch;
     }
     .db-schema__table { 
       width: auto;
       min-width: 1200px; /* 強制最小寬度，確保觸發水平滾動 */
       border-collapse: collapse; 
       background: #fff;
       table-layout: fixed;
     }
     .db-schema__table-header {
       border-bottom: 1px solid #1976d2; 
       padding: 12px 8px; 
       text-align: left; 
       color: #fff; 
       font-weight: 600;
       white-space: nowrap;
       min-width: 150px;
       width: 150px;
     }
     .db-schema__table-cell {
       padding: 8px;
       vertical-align: top;
       min-width: 150px;
       width: 150px;
       max-width: 300px;
       overflow: hidden;
     }
     .cell-content {
       display: flex;
       align-items: flex-start;
       gap: 4px;
     }
     .cell-text {
       flex: 1;
       cursor: pointer;
       word-break: break-all;
       line-height: 1.4;
     }
     .cell-text:hover {
       color: #1976d2;
       text-decoration: underline;
     }
     .cell-text.expanded {
       white-space: pre-wrap;
       max-height: 200px;
       overflow-y: auto;
       background: #f8f9fa;
       padding: 4px;
       border-radius: 4px;
       border: 1px solid #ddd;
     }
     .expand-icon {
       font-size: 16px;
       width: 16px;
       height: 16px;
       cursor: pointer;
       color: #666;
       transition: transform 0.2s ease;
       flex-shrink: 0;
       margin-top: 2px;
     }
     .expand-icon:hover {
       color: #1976d2;
     }
     .expand-icon.rotated {
       transform: rotate(180deg);
     }
     .db-schema__sidebar { 
       width: 220px; 
       background: #f5f5f5; 
       border-right: 1px solid #eee; 
       padding: 0; 
     }
     .db-schema__sidebar-title { 
       margin: 0; 
       padding: 16px; 
       font-size: 18px; 
       background: #f5f5f5; 
       color: #1976d2; 
     }
     .db-schema__sidebar-divider { 
       height: 1px; 
       background: #e0e0e0; 
       margin: 0; 
       border-bottom: 1px solid #e0e0e0; 
     }
     .db-schema__sidebar-list { 
       list-style: none; 
       margin: 0; 
       padding: 0; 
     }
     .db-schema__sidebar-item { 
       padding: 12px 16px; 
       cursor: pointer; 
       border-bottom: 1px solid #eee; 
       background: #f5f5f5; 
       transition: background 0.2s; 
     }
     .db-schema__sidebar-item--active { 
       background: #e3f0fa !important; 
       color: #1976d2; 
     }
     
           /* 滾動條樣式 - 強制顯示水平滾動條 */
     .db-schema__table-container::-webkit-scrollbar {
       height: 12px; /* 增加高度讓滾動條更明顯 */
       width: 12px;
     }
     .db-schema__table-container::-webkit-scrollbar-track {
       background: #f1f1f1;
       border-radius: 6px;
       box-shadow: inset 0 0 2px rgba(0,0,0,0.1);
     }
     .db-schema__table-container::-webkit-scrollbar-thumb {
       background: #888;
       border-radius: 6px;
       border: 2px solid #f1f1f1;
     }
     .db-schema__table-container::-webkit-scrollbar-thumb:hover {
       background: #555;
     }
     .db-schema__table-container::-webkit-scrollbar-corner {
       background: #f1f1f1;
     }
    `
  ]
})
export class DatabaseSchemaComponent implements OnInit {
  tables: string[] = [];
  selectedTable: string | null = null;
  columns: DatabaseColumn[] = [];
  rows: DatabaseRow[] = [];
  loadingRows = false;

  // 分頁相關
  pageIndex = 0;
  pageSize = 10;
  pageSizeOptions = [5, 10, 20, 50, 100];

  get pagedRows() {
    const start = this.pageIndex * this.pageSize;
    return this.rows.slice(start, start + this.pageSize);
  }
  get totalPages() {
    return Math.ceil(this.rows.length / this.pageSize) || 1;
  }

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<string[]>('/api/database/tables').subscribe(tables => {
      this.tables = tables;
      if (tables.length > 0) {
        this.selectTable(tables[0]);
      }
    });
  }

  selectTable(table: string) {
    this.selectedTable = table;
    this.columns = [];
    this.rows = [];
    this.pageIndex = 0;
    this.http.get<DatabaseColumn[]>(`/api/database/tables/${table}/columns`).subscribe(cols => {
      this.columns = cols;
    });
    this.loadingRows = true;
    this.http.get<DatabaseRow[]>(`/api/database/tables/${table}/rows`).subscribe(rows => {
      this.rows = rows;
      this.loadingRows = false;
    }, () => this.loadingRows = false);
  }

  prevPage() {
    if (this.pageIndex > 0) this.pageIndex--;
  }
  nextPage() {
    if (this.pageIndex < this.totalPages - 1) this.pageIndex++;
  }
  onPageSizeChange() {
    this.pageIndex = 0;
  }

  extractContentText(content: string): string {
    if (!content) return '';
    try {
      const matches = Array.from(content.matchAll(/"content"\s*:\s*"(.*?)"/g));
      return matches.map(m => m[1]).join(', ');
    } catch {
      // 🛡️ 記錄解析錯誤但不暴露敏感資訊
      console.warn('內容解析失敗 - 使用預設值');
      return '';
    }
  }

  // 🛡️ 安全的切換展開/收合功能 - 防止 Object Injection
  toggleExpand(row: DatabaseRow, columnName: string): void {
    if (!row || typeof row !== 'object') return;
    
    const safeColumnName = this.sanitizeColumnName(columnName);
    const expandKey = '_expanded_' + safeColumnName;
    
    if (Object.prototype.hasOwnProperty.call(row, expandKey)) {
      // 🛡️ 安全的屬性設置
      const currentValue = this.getSafePropertyValue(row, expandKey, false);
      Object.defineProperty(row, expandKey, {
        value: !currentValue,
        writable: true,
        enumerable: false,
        configurable: true
      });
    } else {
      // 🛡️ 安全的屬性設置
      Object.defineProperty(row, expandKey, {
        value: true,
        writable: true,
        enumerable: false,
        configurable: true
      });
    }
  }

  // 🛡️ 安全的獲取行值 - 防止 Object Injection
  getRowValue(row: DatabaseRow, columnName: string): string {
    if (!row || typeof row !== 'object' || !columnName) return '';
    
    // 只允許安全的欄位名稱
    const safeColumnName = this.sanitizeColumnName(columnName);
    if (safeColumnName !== columnName) {
      console.warn('不安全的欄位名稱被過濾:', columnName);
      return '';
    }
    
    // 🛡️ 安全的屬性存取並轉換為字符串
    const value = this.getSafePropertyValue(row, columnName, '');
    return value != null ? String(value) : '';
  }

  // 🛡️ 安全的獲取展開狀態 - 防止 Object Injection
  getExpandedState(row: DatabaseRow, columnName: string): boolean {
    if (!row || typeof row !== 'object') return false;
    
    const safeColumnName = this.sanitizeColumnName(columnName);
    const expandKey = '_expanded_' + safeColumnName;
    
    // 🛡️ 安全的屬性存取
    const value = this.getSafePropertyValue(row, expandKey, false);
    return !!value;
  }

  // 清理欄位名稱，防止注入攻擊
  private sanitizeColumnName(columnName: string): string {
    if (!columnName || typeof columnName !== 'string') return 'unknown';
    
    return columnName
      .replace(/[^a-zA-Z0-9_]/g, '_') // 只允許字母、數字和底線
      .substring(0, 50); // 限制長度
  }

  // 🛡️ 安全的屬性值獲取 - 防止 Object Injection
  private getSafePropertyValue(obj: DatabaseRow, key: string, defaultValue: unknown = undefined): unknown {
    if (!obj || typeof obj !== 'object' || !key) return defaultValue;
    
    if (Object.prototype.hasOwnProperty.call(obj, key)) {
      const value = Object.getOwnPropertyDescriptor(obj, key)?.value;
      return value !== undefined ? value : defaultValue;
    }
    
    return defaultValue;
  }
} 