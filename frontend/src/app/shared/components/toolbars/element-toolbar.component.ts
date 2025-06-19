import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';

/**
 * 基本工具列元件，用於顯示文字編輯工具列等浮動工具列
 */
@Component({
  selector: 'sn-element-toolbar',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatMenuModule
  ],
  template: `
    <div class="element-toolbar"
         [style.left.px]="position.x"
         [style.top.px]="position.y"
         [style.z-index]="1000">

      <!-- 工具按鈕容器 -->
      <div class="element-toolbar__content">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styleUrls: ['./element-toolbar.component.scss']
})
export class ElementToolbarComponent implements OnInit, OnDestroy {
  @Input() position = { x: 0, y: 0 };
  @Input() targetElement: HTMLElement | null = null;
  @Input() keepOpenWhenTargetSelected = true; // 當目標元素被選中時保持工具列開啟
  @Input() targetSelectedClass = 'selected'; // 目標元素被選中時的 CSS 類別名稱
  @Output() elementChange = new EventEmitter<void>();

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    // 點擊外部關閉工具列
    document.addEventListener('click', this.onDocumentClick.bind(this));

    // 監聽滾動和調整大小事件
    window.addEventListener('resize', this.updatePosition.bind(this));
    window.addEventListener('scroll', this.updatePosition.bind(this));

    // 初始化位置
    if (this.targetElement) {
      setTimeout(() => {
        this.updatePosition();
      }, 0);
    }
  }

  ngOnDestroy() {
    document.removeEventListener('click', this.onDocumentClick.bind(this));
    window.removeEventListener('resize', this.updatePosition.bind(this));
    window.removeEventListener('scroll', this.updatePosition.bind(this));
  }

  private onDocumentClick(event: Event) {
    const target = event.target as HTMLElement;

    // 如果點擊的是工具列本身或選單，不關閉
    if (target.closest('.element-toolbar') || target.closest('.mat-menu-panel')) {
      return;
    }

    // 如果啟用了保持開啟功能
    if (this.keepOpenWhenTargetSelected) {
      // 如果點擊的是目標元素或其子元素，不關閉工具列
      if (this.targetElement && (target === this.targetElement || this.targetElement.contains(target))) {
        return;
      }

      // 檢查目標元素是否仍然被選中
      if (this.isTargetElementSelected()) {
        return;
      }

      // 檢查是否點擊了具有選中狀態的元素
      if (target.closest(`.${this.targetSelectedClass}`) ||
          target.closest('.active') ||
          target.closest('[data-selected="true"]')) {
        return;
      }
    }

    // 其他情況才關閉工具列
    this.elementChange.emit();
  }

  /**
   * 檢查目標元素是否仍然被選中
   */
  private isTargetElementSelected(): boolean {
    if (!this.targetElement) return false;

    return this.targetElement.classList.contains(this.targetSelectedClass) ||
           this.targetElement.classList.contains('active') ||
           this.targetElement.hasAttribute('data-selected') ||
           this.targetElement.getAttribute('data-selected') === 'true';
  }

  /**
   * 更新工具列位置，保持在目標元素上方
   */
  updatePosition() {
    if (!this.targetElement) return;

    const elementRect = this.targetElement.getBoundingClientRect();
    const toolbarHeight = 50; // 工具列固定高度
    const toolbarWidth = 350; // 與TextEditingToolbarComponent相同的寬度估計

    // 計算新位置（水平置中於元素，保持在元素上方）
    let x = elementRect.left + elementRect.width / 2;
    let y = elementRect.top - toolbarHeight - 10;

    // 確保工具列不超出視窗範圍
    const windowWidth = window.innerWidth;
    if (x < 10) x = 10;
    else if (x + toolbarWidth > windowWidth - 10) x = windowWidth - toolbarWidth - 10;

    // 如果頂部空間不足，則顯示在元素下方
    if (y < 10) {
      y = elementRect.bottom + 10;
    }

    this.position = { x, y };
    this.cdr.detectChanges(); // 確保位置更新後UI立即更新
  }
}