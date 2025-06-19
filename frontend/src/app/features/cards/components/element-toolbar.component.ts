import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';

// 文字內容常數
export const ELEMENT_TOOLBAR_TEXT = {
  sections: {
    addElement: {
      title: '添加元素',
      items: {
        text: '文字',
        image: '圖片',
        shape: '形狀',
        qrcode: 'QR碼'
      }
    },
    templates: {
      title: '快速模板',
      items: {
        nameCard: {
          name: '姓名牌',
          description: '基礎姓名牌格式'
        },
        meeting: {
          name: '會議室',
          description: '會議室資訊牌'
        },
        event: {
          name: '活動',
          description: '活動資訊牌'
        }
      }
    },
    layoutTools: {
      title: '排版工具',
      groups: {
        align: {
          label: '對齊',
          actions: {
            left: '靠左對齊',
            center: '居中對齊',
            right: '右對齊'
          }
        },
        layer: {
          label: '圖層',
          actions: {
            front: '移到最前',
            back: '移到最後'
          }
        }
      }
    }
  },
  shapes: {
    rectangle: '矩形',
    circle: '圓形',
    line: '直線'
  }
};

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
  templateUrl: './element-toolbar.component.html',
  styleUrls: ['./element-toolbar.component.scss']
})
export class ElementToolbarComponent {
  @Output() addElement = new EventEmitter<{ type: string, options?: { shapeType?: string } }>();
  @Output() addTemplate = new EventEmitter<string>();
  @Output() alignElements = new EventEmitter<string>();
  @Output() layerAction = new EventEmitter<string>();
  @Output() templateModalOpen = new EventEmitter<void>();
  @Output() backgroundModalOpen = new EventEmitter<void>();
  @Output() imageModalOpen = new EventEmitter<void>();
  @Output() openShapeSelectorModal = new EventEmitter<void>();

  // 文字常數
  text = ELEMENT_TOOLBAR_TEXT;

  // 添加基本元素
  addTextElement() {
    this.addElement.emit({ type: 'text' });
  }

  addImageElement() {
    this.imageModalOpen.emit();
  }

  addShapeElement(shape: string) {
    this.addElement.emit({ type: 'shape', options: { shapeType: shape } });
  }

  addQRCodeElement() {
    this.addElement.emit({ type: 'qrcode' });
  }

  // 添加模板
  addNameCardTemplate() {
    this.addTemplate.emit('namecard');
  }

  addMeetingTemplate() {
    this.addTemplate.emit('meeting');
  }

  addEventTemplate() {
    this.addTemplate.emit('event');
  }

  // 排版工具
  alignLeft() {
    this.alignElements.emit('left');
  }

  alignCenter() {
    this.alignElements.emit('center');
  }

  alignRight() {
    this.alignElements.emit('right');
  }

  bringToFront() {
    this.layerAction.emit('front');
  }

  sendToBack() {
    this.layerAction.emit('back');
  }

  // 新增方法
  openTemplateModal() {
    this.templateModalOpen.emit();
  }

  openBackgroundModal() {
    this.backgroundModalOpen.emit();
  }

  openShapeSelector() {
    this.openShapeSelectorModal.emit();
  }
}
