import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CanvasData, CanvasElement } from '../models/card-design.models';

@Component({
  selector: 'sn-card-preview',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card-preview-canvas" [style.width.px]="canvasData?.width || 240" [style.height.px]="canvasData?.height || 150" [style.position]="'relative'" [style.background]="canvasData?.background || '#f7f7f9'">
      <ng-container *ngFor="let el of canvasData?.elements">
        <div *ngIf="el.type === 'text'"
             [style.position]="'absolute'"
             [style.left.px]="el.position.x"
             [style.top.px]="el.position.y"
             [style.width.px]="el.size.width"
             [style.height.px]="el.size.height"
             [style.fontSize.px]="el.style.fontSize"
             [style.color]="el.style.color"
             [style.fontWeight]="el.style.fontWeight"
             [style.fontFamily]="el.style.fontFamily"
             [style.textAlign]="el.style.textAlign"
             [style.background]="el.style.backgroundColor || 'transparent'"
             [style.borderRadius.px]="el.style.borderRadius || 0"
             [style.padding.px]="el.style.padding || 0"
             [style.overflow]="'hidden'">
          {{ el.content }}
        </div>
        <img *ngIf="el.type === 'image'"
             [src]="el.src"
             [alt]="el.alt"
             [style.position]="'absolute'"
             [style.left.px]="el.position.x"
             [style.top.px]="el.position.y"
             [style.width.px]="el.size.width"
             [style.height.px]="el.size.height"
             [style.borderRadius.px]="el.style.borderRadius || 0"
             [style.opacity]="el.style.opacity || 1"
             style="object-fit:cover;" />
        <div *ngIf="el.type === 'shape'"
             [style.position]="'absolute'"
             [style.left.px]="el.position.x"
             [style.top.px]="el.position.y"
             [style.width.px]="el.size.width"
             [style.height.px]="el.size.height"
             [style.background]="el.style.backgroundColor"
             [style.borderRadius.px]="el.shapeType === 'circle' ? 9999 : (el.style.borderRadius || 0)"
             [style.border]="el.style.borderWidth ? (el.style.borderWidth + 'px solid ' + el.style.borderColor) : 'none'">
        </div>
      </ng-container>
    </div>
  `,
  styleUrls: ['./card-preview.component.scss']
})
export class CardPreviewComponent {
  @Input() canvasData?: CanvasData;
} 