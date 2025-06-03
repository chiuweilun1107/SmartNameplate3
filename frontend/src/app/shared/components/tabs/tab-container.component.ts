import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TagButtonComponent } from '../tags/tag-button.component';

export interface TabItem {
  key: string;
  label: string;
  icon?: string;
  disabled?: boolean;
}

@Component({
  selector: 'sn-tab-container',
  standalone: true,
  imports: [CommonModule, TagButtonComponent],
  template: `
    <div class="tab-container">
      <div class="tab-container__tabs">
        <div class="tab-container__tabs-left">
          <sn-tag-button
            *ngFor="let tab of tabs"
            [label]="tab.label"
            [icon]="tab.icon"
            [isActive]="activeTab === tab.key"
            [isDisabled]="!!tab.disabled"
            (tagClick)="switchTab(tab.key)">
          </sn-tag-button>
        </div>
        <div class="tab-container__tabs-right">
          <ng-content select="[slot=tab-actions]"></ng-content>
        </div>
      </div>

      <div class="tab-container__content">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styleUrls: ['./tab-container.component.scss']
})
export class TabContainerComponent {
  @Input() tabs: TabItem[] = [];
  @Input() activeTab = '';
  @Output() tabChange = new EventEmitter<string>();

  switchTab(tabKey: string) {
    if (this.activeTab !== tabKey) {
      this.activeTab = tabKey;
      this.tabChange.emit(tabKey);
    }
  }
} 