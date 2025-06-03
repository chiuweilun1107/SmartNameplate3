import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ShapeOption {
  type: 'rectangle' | 'circle' | 'line' | 'triangle' | 'star' | 'polygon';
  name: string;
  icon: string;
  description: string;
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
    <div class="modal-overlay" (click)="onOverlayClick($event)" *ngIf="isVisible">
      <div class="modal-container" (click)="$event.stopPropagation()">
        <div class="modal-header">
          <h2 class="modal-title">選擇形狀</h2>
          <button mat-icon-button class="modal-close-btn" (click)="closeModal()">
            <mat-icon>close</mat-icon>
          </button>
        </div>

        <div class="modal-content">
          <div class="shape-grid">
            <div
              *ngFor="let shape of shapes"
              class="shape-item"
              [class.selected]="selectedShape?.type === shape.type"
              (click)="selectShape(shape)">
              <div class="shape-preview">
                <mat-icon>{{ shape.icon }}</mat-icon>
              </div>
              <div class="shape-info">
                <h3>{{ shape.name }}</h3>
                <p>{{ shape.description }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button 
            mat-raised-button 
            color="primary" 
            [disabled]="!selectedShape"
            (click)="confirm()">
            確認
          </button>
          <button mat-button (click)="closeModal()">
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
  @Input() currentShapeType: string = 'rectangle';
  @Output() shapeSelected = new EventEmitter<ShapeOption>();
  @Output() close = new EventEmitter<void>();

  selectedShape: ShapeOption | null = null;

  shapes: ShapeOption[] = [
    {
      type: 'rectangle',
      name: '矩形',
      icon: 'crop_square',
      description: '基本矩形形狀'
    },
    {
      type: 'circle',
      name: '圓形',
      icon: 'circle',
      description: '圓形或橢圓形'
    },
    {
      type: 'line',
      name: '直線',
      icon: 'remove',
      description: '直線或分隔線'
    },
    {
      type: 'triangle',
      name: '三角形',
      icon: 'change_history',
      description: '三角形形狀'
    },
    {
      type: 'star',
      name: '星形',
      icon: 'star',
      description: '五角星形狀'
    },
    {
      type: 'polygon',
      name: '多邊形',
      icon: 'hexagon',
      description: '多邊形形狀'
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
    this.close.emit();
  }

  onOverlayClick(event: Event): void {
    if (event.target === event.currentTarget) {
      this.closeModal();
    }
  }
} 