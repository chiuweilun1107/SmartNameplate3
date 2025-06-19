import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { timeout } from 'rxjs/operators';
import {
  Group,
  GroupDetail,
  Card,
  Device,
  BluetoothDevice,
  CreateGroup,
  UpdateGroup,
  AddCardToGroup,
  ConnectDevice,
  UpdateDevice,
  DeployCard
} from '../models/deploy.models';

@Injectable({
  providedIn: 'root'
})
export class DeployService {
  private readonly apiUrl = '/api';

  constructor(private http: HttpClient) {}

  // Groups API
  getGroups(): Observable<Group[]> {
    return this.http.get<Group[]>(`${this.apiUrl}/groups`);
  }

  getGroup(id: number): Observable<GroupDetail> {
    return this.http.get<GroupDetail>(`${this.apiUrl}/groups/${id}`);
  }

  createGroup(group: CreateGroup): Observable<Group> {
    return this.http.post<Group>(`${this.apiUrl}/groups`, group);
  }

  updateGroup(id: number, group: UpdateGroup): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/groups/${id}`, group);
  }

  deleteGroup(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/groups/${id}`);
  }

  addCardToGroup(groupId: number, cardId: number): Observable<void> {
    const addCardDto: AddCardToGroup = { cardId };
    return this.http.post<void>(`${this.apiUrl}/groups/${groupId}/cards`, addCardDto);
  }

  removeCardFromGroup(groupId: number, cardId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/groups/${groupId}/cards/${cardId}`);
  }

  // Cards API
  getCards(): Observable<Card[]> {
    return this.http.get<Card[]>(`${this.apiUrl}/cards`);
  }

  // Bluetooth API
  scanBluetoothDevices(): Observable<BluetoothDevice[]> {
    return this.http.get<BluetoothDevice[]>(`${this.apiUrl}/bluetooth/scan`);
  }

  connectDevice(device: ConnectDevice): Observable<Device> {
    return this.http.post<Device>(`${this.apiUrl}/bluetooth/connect`, device);
  }

  getDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.apiUrl}/bluetooth/devices`);
  }

  updateDevice(id: number, device: UpdateDevice): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/bluetooth/devices/${id}`, device);
  }

  deployCardToDevice(deviceId: number, deploy: DeployCard): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/bluetooth/devices/${deviceId}/deploy`, deploy);
  }

  castImageToDevice(deviceId: number, deploy: DeployCard): Observable<{ message: string }> {
    // 投圖需要較長時間，設置120秒超時（雙面投圖需要更多時間）
    return this.http.post<{ message: string }>(`${this.apiUrl}/bluetooth/devices/${deviceId}/cast`, deploy)
      .pipe(timeout(120000)); // 120秒超時
  }

  removeDevice(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/bluetooth/devices/${id}`);
  }

  // 新增：更新設備 customIndex
  updateDeviceIndex(device: Device, customIndex: number | null): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/bluetooth/devices/${device.id}`, {
      name: device.name,
      groupId: device.groupId ?? null,
      customIndex: customIndex
    });
  }
}