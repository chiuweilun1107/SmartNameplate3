export interface Group {
  id: number;
  name: string;
  description?: string;
  color?: string;
  cardCount: number;
  deviceCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface GroupDetail {
  id: number;
  name: string;
  description?: string;
  color?: string;
  cards: Card[];
  devices: Device[];
  createdAt: string;
  updatedAt: string;
}

export interface Card {
  id: number;
  name: string;
  description?: string;
  status: string;
  thumbnail?: string;
  thumbnailA?: string;
  thumbnailB?: string;
  contentA?: Record<string, unknown>;
  contentB?: Record<string, unknown>;
  isSameBothSides: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface Device {
  id: number;
  name: string;
  bluetoothAddress: string;
  status: string;
  currentCardId?: number;
  currentCardName?: string;
  groupId?: number;
  groupName?: string;
  lastConnected: string;
  createdAt: string;
  updatedAt: string;
  customIndex?: number;
}

export interface BluetoothDevice {
  name: string;
  bluetoothAddress: string;
  originalAddress?: string;
  signalStrength: number;
  isConnected: boolean;
  deviceType: string;
}

export interface CreateGroup {
  name: string;
  description?: string;
  color?: string;
}

export interface UpdateGroup {
  name: string;
  description?: string;
  color?: string;
}

export interface AddCardToGroup {
  cardId: number;
}

export interface ConnectDevice {
  name: string;
  bluetoothAddress: string;
  originalAddress?: string;
}

export interface UpdateDevice {
  name: string;
  groupId?: number;
}

export interface DeployCard {
  cardId: number;
  side?: number; // 0=AB同時刷新, 1=A面, 2=B面 (預設2)
} 