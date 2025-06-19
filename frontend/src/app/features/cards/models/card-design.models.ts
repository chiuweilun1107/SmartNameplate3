// 元素類型定義
export interface Position {
  x: number;
  y: number;
}

export interface Size {
  width: number;
  height: number;
}

export interface TextElement {
  type: 'text';
  id: string;
  content: string;
  position: Position;
  size: Size;
  style: {
    fontSize: number;
    fontFamily: string;
    fontWeight: 'normal' | 'bold';
    fontStyle?: 'normal' | 'italic';
    textDecoration?: 'none' | 'underline' | 'line-through';
    color: string;
    textAlign: 'left' | 'center' | 'right';
    backgroundColor?: string;
    borderRadius?: number;
    borderWidth?: number;
    borderColor?: string;
    padding?: number;
    tag?: string;
  };
  zIndex: number;
}

export interface ImageElement {
  type: 'image';
  id: string;
  src: string;
  alt: string;
  position: Position;
  size: Size;
  style: {
    borderRadius?: number;
    borderWidth?: number;
    borderColor?: string;
    opacity?: number;
    filter?: string;
  };
  zIndex: number;
}

export interface ShapeElement {
  type: 'shape';
  id: string;
  shapeType: 'rectangle' | 'circle' | 'line' | 'triangle' | 'star' | 'polygon';
  position: Position;
  size: Size;
  style: {
    backgroundColor: string;
    borderColor?: string;
    borderWidth?: number;
    borderRadius?: number;
  };
  zIndex: number;
}

export interface QRCodeElement {
  type: 'qrcode';
  id: string;
  data: string;
  position: Position;
  size: Size;
  style: {
    backgroundColor: string;
    foregroundColor: string;
    borderColor?: string;
    borderWidth?: number;
    borderRadius?: number;
  };
  margin?: number;
  errorCorrectionLevel?: 'L' | 'M' | 'Q' | 'H';
  zIndex: number;
}

export type CanvasElement = TextElement | ImageElement | ShapeElement | QRCodeElement;

export interface CanvasData {
  elements: CanvasElement[];
  background: string;
  width: number;
  height: number;
}

export interface CardDesign {
  id: string;
  name: string;
  description?: string;
  A: CanvasData;
  B: CanvasData;
  createdAt: Date;
  updatedAt: Date;
  createdBy: string;
  isTemplate: boolean;
}

// 協作編輯相關
export interface CollaborationUser {
  id: string;
  name: string;
  avatar?: string;
  color: string;
  cursor?: Position;
  selectedElement?: string;
}

export interface CollaborationAction {
  type: 'element_add' | 'element_update' | 'element_delete' | 'element_move' | 'cursor_move';
  userId: string;
  timestamp: Date;
  data: Position | CanvasElement | string | Record<string, unknown>;
}
