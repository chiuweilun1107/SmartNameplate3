import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

export interface ShapeOption {
  type: 'rectangle' | 'circle' | 'line' | 'triangle' | 'star' | 'polygon';
  name: string;
  icon: string;
  description: string;
  svg?: string;
}

@Component({
  selector: 'sn-shape-selector-modal',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="modal-overlay" (click)="onOverlayClick($event)"
      (keydown.enter)="onOverlayClick($event)"
      (keydown.space)="onOverlayClick($event)"
      tabindex="0" role="button" *ngIf="isVisible">
      <div class="modal-container" 
           (click)="$event.stopPropagation()"
           (keydown.enter)="$event.stopPropagation()"
           (keydown.space)="$event.stopPropagation()"
           tabindex="0" 
           role="dialog"
           aria-label="形狀選擇對話框">
        <div class="modal-header">
          <h2 class="modal-title">選擇形狀</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()"
            (keydown.enter)="closeModal()"
            (keydown.space)="closeModal()"
            tabindex="0" role="button">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="shape-grid">
            <div
              *ngFor="let shape of shapes"
              class="shape-item"
              [class.selected]="selectedShape === shape"
              (click)="selectShape(shape)"
              (keydown.enter)="selectShape(shape)"
              (keydown.space)="selectShape(shape)"
              tabindex="0"
              role="button"
              [attr.aria-label]="'選擇形狀 ' + shape.name">
              <div class="shape-preview" [innerHTML]="getSafeSvg(shape.svg)"></div>
              <span class="shape-name">{{ shape.name }}</span>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button 
            mat-raised-button 
            color="primary" 
            [disabled]="!selectedShape"
            (click)="confirm()"
            (keydown.enter)="confirm()"
            (keydown.space)="confirm()"
            tabindex="0" role="button">
            確認
          </button>
          <button mat-button (click)="closeModal()"
            (keydown.enter)="closeModal()"
            (keydown.space)="closeModal()"
            tabindex="0" role="button">
            取消
          </button>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./shape-selector-modal.component.scss']
})
export class ShapeSelectorModalComponent implements OnInit {
  @Input() isVisible = false;
  @Input() currentShapeType = 'rectangle';
  @Output() shapeSelected = new EventEmitter<ShapeOption>();
  @Output() modalClose = new EventEmitter<void>();

  selectedShape: ShapeOption | null = null;

  constructor(private sanitizer: DomSanitizer) {}

  shapes: ShapeOption[] = [
    {
      type: 'rectangle',
      name: '矩形',
      icon: 'crop_square',
      description: '基本矩形形狀',
      svg: '<svg viewBox="0 0 100 100" width="60" height="60"><rect x="15" y="25" width="70" height="50" fill="#2196F3" stroke="#1976D2" stroke-width="2" rx="4"/></svg>'
    },
    {
      type: 'circle',
      name: '圓形',
      icon: 'circle',
      description: '圓形或橢圓形',
      svg: '<svg viewBox="0 0 100 100" width="60" height="60"><circle cx="50" cy="50" r="35" fill="#4CAF50" stroke="#388E3C" stroke-width="2"/></svg>'
    },
    {
      type: 'line',
      name: '直線',
      icon: 'remove',
      description: '直線或分隔線',
      svg: '<svg viewBox="0 0 100 100" width="60" height="60"><line x1="15" y1="50" x2="85" y2="50" stroke="#FF9800" stroke-width="4" stroke-linecap="round"/></svg>'
    },
    {
      type: 'triangle',
      name: '三角形',
      icon: 'change_history',
      description: '三角形形狀',
      svg: '<svg viewBox="0 0 100 100" width="60" height="60"><polygon points="50,15 85,75 15,75" fill="#9C27B0" stroke="#7B1FA2" stroke-width="2" stroke-linejoin="round"/></svg>'
    },
    {
      type: 'star',
      name: '星形',
      icon: 'star',
      description: '五角星形狀',
      svg: '<svg viewBox="0 0 100 100" width="60" height="60"><polygon points="50,10 61,35 85,35 67,53 73,78 50,65 27,78 33,53 15,35 39,35" fill="#FFC107" stroke="#F57C00" stroke-width="2" stroke-linejoin="round"/></svg>'
    },
    {
      type: 'polygon',
      name: '多邊形',
      icon: 'hexagon',
      description: '多邊形形狀',
      svg: '<svg viewBox="0 0 100 100" width="60" height="60"><polygon points="50,15 75,30 75,70 50,85 25,70 25,30" fill="#00BCD4" stroke="#0097A7" stroke-width="2" stroke-linejoin="round"/></svg>'
    }
  ];

  ngOnInit(): void {
    // 預設選擇當前形狀
    this.selectedShape = this.shapes.find(s => s.type === this.currentShapeType) || this.shapes[0];
  }

  selectShape(shape: ShapeOption): void {
    this.selectedShape = shape;
  }

  confirm(): void {
    if (this.selectedShape) {
      this.shapeSelected.emit(this.selectedShape);
      this.closeModal();
    }
  }

  closeModal(): void {
    this.modalClose.emit();
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }

  getSafeSvg(svg?: string): SafeHtml {
    if (!svg) return '';
    return this.sanitizer.bypassSecurityTrustHtml(svg);
  }
} 