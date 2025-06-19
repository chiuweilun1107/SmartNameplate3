import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { DeployService } from '../services/deploy.service';
import { Group, GroupDetail, Card, Device, BluetoothDevice } from '../models/deploy.models';
import { NotificationService } from '../../../shared/services/notification.service';
import { CryptoService } from '../../../core/services/crypto.service';
import { TagButtonComponent } from '../../../shared/components/tags/tag-button.component';
import { TemplateModalComponent } from '../../../shared/components/modals/template-modal.component';
import { TabContainerComponent, TabItem } from '../../../shared/components/tabs/tab-container.component';
import { GenericSelectionModalComponent, SelectionItem } from '../../../shared/components/modals/generic-selection-modal.component';
import { CardSelectionItemComponent } from '../../../shared/components/cards/card-selection-item.component';
import { GroupSelectionItemComponent } from '../../../shared/components/groups/group-selection-item.component';
import { DeleteButtonComponent } from '../../../shared/components/delete-button/delete-button.component';

// 文字內容常數
export const DEPLOY_TEXT = {
  title: '管理桌牌群組與設備部署',
  subtitle: '',
  tabs: {
    cards: '自選投圖',
    groups: '群組投圖'
  },
  sections: {
    group: {
      title: '選擇群組',
      empty: {
        noGroup: '尚無任何群組',
        createFirst: '請先到管理頁面創建群組'
      }
    },
    cards: {
      title: '已選取',
      cardSuffix: '張圖卡',
      selectBtn: '選擇圖卡',
      empty: {
        noCards: '尚未選擇任何圖卡',
        pleaseSelect: '請點擊「選擇圖卡」按鈕來選擇要部署的圖卡'
      }
    },
    devices: {
      title: '已選取',
      deviceSuffix: '台桌牌',
      castBtn: '投圖到所有設備',
      addBtn: '新增桌牌',
      empty: {
        noDevices: '尚未連接任何桌牌設備',
        pleaseAdd: '請點擊「新增桌牌」按鈕來搜尋附近的藍牙設備'
      },
      status: {
        connected: '已連接',
        disconnected: '未連接',
        error: '錯誤',
        connecting: '連接中',
        casting: '投圖中',
        current: '當前:'
      },
      actions: {
        cast: '投圖',
        deploy: '部署',
        remove: '移除'
      }
    },
    modal: {
      card: {
        title: '請選擇圖卡',
        closeBtn: '關閉',
        confirmBtn: '確認選擇'
      },
      bluetooth: {
        title: '選擇要新增的設備',
        scanBtn: '掃描藍牙設備',
        stopScanBtn: '停止掃描',
        scanStatus: '正在掃描藍牙設備...',
        noDevices: '未找到藍牙設備',
        closeBtn: '關閉',
        confirmBtn: '連接'
      }
    }
  }
};

@Component({
  selector: 'sn-deploy',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule, TagButtonComponent, TemplateModalComponent, TabContainerComponent, GenericSelectionModalComponent, CardSelectionItemComponent, GroupSelectionItemComponent, DeleteButtonComponent],
  templateUrl: './deploy.component.html',
  styleUrls: ['./deploy.component.scss']
})
export class DeployComponent implements OnInit {
  groups: Group[] = [];
  selectedGroup: GroupDetail | null = null;
  selectedCards: Card[] = [];
  availableCards: Card[] = [];
  devices: Device[] = [];
  
  // 標籤切換
  activeTab: 'cards' | 'groups' = 'cards';
  
  // 頁籤配置
  cardTabs: TabItem[] = [
    { key: 'cards', label: '自選投圖', icon: 'credit_card' },
    { key: 'groups', label: '群組投圖', icon: 'group_work' }
  ];

  // 藍牙掃描相關
  isScanning = false;
  bluetoothDevices: BluetoothDevice[] = [];
  showBluetoothModal = false;
  selectedDevices = new Set<string>();
  
  // 卡片選擇相關
  showCardSelectionModal = false;
  
  // 群組選擇相關
  showGroupSelectionModal = false;
  
  editIndex: string | null = null;
  
  // 文字常數
  text = DEPLOY_TEXT;
  
  // 部署卡片彈窗狀態
  showDeployCardModalFor: number|null = null;
  
  // 設備管理tab切換
  activeDeviceTab: 'deploy' | 'add' = 'deploy';
  
  // 設備頁籤配置
  deviceTabs: TabItem[] = [
    { key: 'deploy', label: '部署圖卡', icon: 'cloud_upload' },
    { key: 'add', label: '新增設備', icon: 'add' }
  ];
  
  constructor(
    private deployService: DeployService,
    private notificationService: NotificationService,
    private cryptoService: CryptoService
  ) {}

  ngOnInit() {
    this.loadGroups();
    this.loadCards();
    this.loadDevices();
    // 🔧 新增：載入已保存的圖卡選擇
    this.loadSavedCardSelection();
  }

  // 標籤切換
  switchTab(tab: 'cards' | 'groups') {
    this.activeTab = tab;
  }

  // 設備頁籤切換
  onDeviceTabChange(tabKey: string) {
    this.activeDeviceTab = tabKey as 'deploy' | 'add';
  }

  // 卡片頁籤切換
  onCardTabChange(tabKey: string) {
    this.activeTab = tabKey as 'cards' | 'groups';
  }

  loadGroups() {
    this.deployService.getGroups().subscribe({
      next: (groups) => {
        this.groups = groups;
        if (groups.length > 0) {
          this.selectGroup(groups[0]);
        }
      },
      error: (error) => console.error('載入群組失敗:', error)
    });
  }

  loadCards() {
    this.deployService.getCards().subscribe({
      next: (cards) => {
        this.availableCards = cards;
      },
      error: (error) => console.error('載入卡片失敗:', error)
    });
  }

  loadDevices() {
    this.deployService.getDevices().subscribe({
      next: (devices) => {
        this.devices = devices;
        this.sortDevicesByCustomIndex();
        // 自動同步 deployedCards 狀態
        this.deployedCards.clear();
        for (const device of this.devices) {
          if (device.currentCardId) {
            this.deployedCards.add(`${device.id}-${device.currentCardId}`);
          }
        }
      },
      error: (error) => console.error('載入設備失敗:', error)
    });
  }

  selectGroup(group: Group) {
    this.deployService.getGroup(group.id).subscribe({
      next: (groupDetail) => {
        this.selectedGroup = groupDetail;
        this.selectedCards = groupDetail.cards;
      },
      error: (error) => console.error('載入群組詳情失敗:', error)
    });
  }

  // 開啟卡片選擇彈窗
  openCardSelection() {
    console.log('開啟卡片選擇彈窗', {
      selectedGroup: this.selectedGroup,
      selectedCards: this.selectedCards,
      availableCards: this.availableCards
    });
    this.tempSelectedCardIds = this.selectedCards.map(card => card.id);
    this.showCardSelectionModal = true;
  }

  // 關閉卡片選擇彈窗
  closeCardSelection() {
    this.showCardSelectionModal = false;
    this.tempSelectedCardIds = [];
  }

  // 開啟群組選擇
  openGroupSelection() {
    console.log('開啟群組選擇功能');
    this.showGroupSelectionModal = true;
  }

  // 關閉群組選擇彈窗
  closeGroupSelection() {
    this.showGroupSelectionModal = false;
  }

  // 選擇群組（通用模態彈窗單選回調）
  onGroupSelection(group: SelectionItem): void {
    // 將SelectionItem轉換回Group類型
    const selectedGroup = this.groups.find(g => g.id === group.id);
    if (selectedGroup) {
      this.selectGroup(selectedGroup);
      console.log('選擇群組:', selectedGroup.name);
    }
  }

  // 選擇群組
  selectGroupFromModal(group: Group) {
    this.selectGroup(group);
    this.closeGroupSelection();
    console.log('選擇群組:', group.name);
  }

  // 卡片選擇狀態
  tempSelectedCardIds: number[] = [];
  
  // 轉換後的選擇項目
  get cardSelectionItems(): SelectionItem[] {
    return this.availableCards.map(card => ({
      ...card,
      id: card.id,
      name: card.name,
      description: card.description
    }));
  }
  
  get groupSelectionItems(): SelectionItem[] {
    return this.groups.map(group => ({
      ...group,
      id: group.id,
      name: group.name,
      description: group.description
    }));
  }
  
  // 追蹤部署狀態
  deployedCards = new Set<string>();

  // 檢查選擇是否有變更
  hasSelectionChanged(): boolean {
    const currentCardIds = this.selectedCards.map(card => card.id).sort();
    const tempCardIds = [...this.tempSelectedCardIds].sort();
    
    if (currentCardIds.length !== tempCardIds.length) {
      return true;
    }
    
    return !currentCardIds.every((id, index) => id === tempCardIds[index]);
  }

  // 🛡️ 安全的確認選擇卡片（通用模態彈窗回調）- 防止 Object Injection
  onCardSelectionChange(selectedCards: SelectionItem[]): void {
    this.tempSelectedCardIds = selectedCards
      .filter(card => card && typeof card === 'object' && 'id' in card && typeof card.id === 'number')
      .map(card => card.id as number)
      .filter(id => id > 0);
    this.confirmCardSelection();
  }

  // 處理單個卡片選擇變更
  onSingleCardSelectionChange(card: Card, isSelected: boolean): void {
    if (isSelected) {
      if (!this.tempSelectedCardIds.includes(card.id)) {
        this.tempSelectedCardIds.push(card.id);
      }
    } else {
      this.tempSelectedCardIds = this.tempSelectedCardIds.filter(id => id !== card.id);
    }
  }

  // 處理卡片點擊切換
  onCardToggle(card: Card): void {
    const isSelected = this.tempSelectedCardIds.includes(card.id);
    this.onSingleCardSelectionChange(card, !isSelected);
  }

  // 確認選擇卡片
  confirmCardSelection() {
    if (this.tempSelectedCardIds.length === 0) {
      console.log('沒有選擇任何卡片，將清空選擇');
      this.selectedCards = [];
    } else {
      console.log(`選中了 ${this.tempSelectedCardIds.length} 張卡片`);
      
      // 更新選擇的卡片
      this.selectedCards = this.availableCards.filter(card => 
        this.tempSelectedCardIds.includes(card.id)
      );
    }
    
    // 🔧 新增：保存選擇的圖卡到 localStorage
    this.saveCardSelection();
    
    // 🔧 修正：確認選擇後自動關閉彈跳視窗
    this.closeCardSelection();
  }

  // 🔧 新增：保存圖卡選擇到 localStorage
  private saveCardSelection() {
    try {
      const selectedCardIds = this.selectedCards.map(card => card.id);
      localStorage.setItem('deploy_selected_cards', JSON.stringify(selectedCardIds));
      console.log('已保存圖卡選擇:', selectedCardIds);
    } catch (error) {
      console.error('保存圖卡選擇失敗:', error);
    }
  }

  // 🔧 新增：從 localStorage 載入圖卡選擇
  private loadSavedCardSelection() {
    try {
      const savedCardIds = localStorage.getItem('deploy_selected_cards');
      if (savedCardIds) {
        const cardIds: number[] = JSON.parse(savedCardIds);
        console.log('載入已保存的圖卡選擇:', cardIds);
        
        // 等待 availableCards 載入完成後再設置選擇
        this.waitForCardsAndSetSelection(cardIds);
      }
    } catch (error) {
      console.error('載入圖卡選擇失敗:', error);
    }
  }

  // 🔧 新增：等待卡片載入完成後設置選擇
  private waitForCardsAndSetSelection(cardIds: number[]) {
    // 使用 setTimeout 確保 availableCards 已經載入
    const checkAndSet = () => {
      if (this.availableCards.length > 0) {
        this.selectedCards = this.availableCards.filter(card => 
          cardIds.includes(card.id)
        );
        console.log('已恢復圖卡選擇:', this.selectedCards.map(c => c.name));
      } else {
        // 如果還沒載入完成，再等一下
        setTimeout(checkAndSet, 100);
      }
    };
    setTimeout(checkAndSet, 100);
  }



  // 開啟藍牙掃描
  openBluetoothScan() {
    this.showBluetoothModal = true;
    this.scanForDevices();
  }

  // 關閉藍牙掃描彈窗
  closeBluetoothModal() {
    this.showBluetoothModal = false;
    this.bluetoothDevices = [];
    this.selectedDevices.clear();
  }

  // 掃描藍牙設備
  scanForDevices() {
    this.isScanning = true;
    this.deployService.scanBluetoothDevices().subscribe({
      next: (devices) => {
        this.bluetoothDevices = devices;
        this.isScanning = false;
      },
      error: (error) => {
        console.error('藍牙掃描失敗:', error);
        this.isScanning = false;
      }
    });
  }

  // 切換設備選擇狀態
  toggleDeviceSelection(device: BluetoothDevice, event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.checked) {
      this.selectedDevices.add(device.bluetoothAddress);
    } else {
      this.selectedDevices.delete(device.bluetoothAddress);
    }
  }

  // 獲取已選擇設備的ID陣列
  getSelectedDeviceIds(): string[] {
    return Array.from(this.selectedDevices.keys());
  }

  // 當彈窗選擇變更時的處理
  onDeviceSelectionChange(selectedDevices: BluetoothDevice[]): void {
    this.selectedDevices.clear();
    selectedDevices.forEach(device => {
      if (device.bluetoothAddress) {
        this.selectedDevices.add(device.bluetoothAddress);
      }
    });
    this.confirmDeviceSelection();
  }

  // 從卡片式設備選擇器切換選擇狀態
  toggleDeviceSelectionFromCard(device: BluetoothDevice): void {
    if (this.selectedDevices.has(device.bluetoothAddress)) {
      this.selectedDevices.delete(device.bluetoothAddress);
    } else {
      this.selectedDevices.add(device.bluetoothAddress);
    }
  }

  // 獲取信號強度的CSS類
  getSignalStrengthClass(signalStrength: number): string {
    if (signalStrength > -50) return 'signal-strong';
    if (signalStrength > -70) return 'signal-medium';
    return 'signal-weak';
  }

  // 確認選擇的設備並保存到資料庫
  confirmDeviceSelection() {
    if (this.selectedDevices.size === 0) {
      return;
    }

    const selectedDeviceList = this.bluetoothDevices.filter(device => 
      this.selectedDevices.has(device.bluetoothAddress)
    );

    console.log('開始連接選中的設備:', selectedDeviceList.map(d => d.name));

    let completedConnections = 0;
    let successfulConnections = 0;
    const totalConnections = selectedDeviceList.length;

    selectedDeviceList.forEach(device => {
      this.deployService.connectDevice({
        name: device.name,
        bluetoothAddress: device.bluetoothAddress,
        originalAddress: device.originalAddress
      }).subscribe({
        next: () => {
          completedConnections++;
          successfulConnections++;
          console.log(`設備 "${device.name}" 連接成功`);
          
          if (completedConnections === totalConnections) {
            this.finishDeviceConnection(successfulConnections, totalConnections - successfulConnections);
          }
        },
        error: (error) => {
          completedConnections++;
          // 🛡️ 修復Object Injection漏洞：使用安全的日誌記錄
          console.error('設備連接失敗:', { 
            deviceName: String(device?.name || 'unknown'), 
            errorMessage: String(error?.message || '未知錯誤')
          });
          
          if (completedConnections === totalConnections) {
            this.finishDeviceConnection(successfulConnections, totalConnections - successfulConnections);
          }
        }
      });
    });
  }

  private finishDeviceConnection(successful: number, failed: number) {
    console.log(`設備連接完成: ${successful} 成功, ${failed} 失敗`);
    this.loadDevices(); // 重新載入設備列表
    this.closeBluetoothModal();
  }

  // 連接藍牙設備 (保留舊方法以防其他地方使用)
  connectBluetoothDevice(device: BluetoothDevice) {
    this.deployService.connectDevice({
      name: device.name,
      bluetoothAddress: device.bluetoothAddress
    }).subscribe({
      next: (connectedDevice) => {
        console.log('設備連接成功:', connectedDevice);
        this.loadDevices(); // 重新載入設備列表
        this.closeBluetoothModal();
      },
      error: (error) => console.error('設備連接失敗:', error)
    });
  }

  // 投圖到設備
  castImageToDevice(device: Device) {
    if (!device.currentCardId) {
      console.error(`設備 ${device.name} 沒有部署任何卡片`);
      this.notificationService.warning(
        '無法投圖',
        `設備 "${device.name}" 沒有部署任何卡片，請先部署卡片再投圖`
      );
      return;
    }

    console.log(`投圖到設備 ${device.name}: ${device.currentCardName || '未知卡片'}`);
    console.log('🔍 投圖請求詳細資訊:', {
      deviceId: device.id,
      deviceName: device.name,
      currentCardId: device.currentCardId,
      currentCardName: device.currentCardName,
      requestPayload: { cardId: device.currentCardId, side: 2 }
    });
    
    // 🎯 顯示真實投圖進度
    const notificationId = this.notificationService.loading(
      '正在投圖...',
      `準備投圖A面 "${device.currentCardName || '卡片'}" 到 ${device.name}`,
      true
    );
    
    // 🎯 追蹤真實投圖狀態
    let currentPhase = 'preparing'; // preparing -> sideA -> sideB -> complete
    let currentProgress = 0;
    
    // 🎯 基於實際投圖時間的進度更新（每個面約25秒，總共約55秒）
    const progressTimer = setInterval(() => {
      switch (currentPhase) {
        case 'preparing':
          currentProgress += 2;
          if (currentProgress >= 10) {
            currentPhase = 'sideA';
            this.notificationService.updateProgress(notificationId, currentProgress, '正在投圖A面...');
          } else {
            this.notificationService.updateProgress(notificationId, currentProgress, '準備投圖A面...');
          }
          break;
          
        case 'sideA':
          currentProgress += 1.5; // A面約25秒完成（10%-50%）
          if (currentProgress >= 50) {
            currentPhase = 'sideB';
            this.notificationService.updateProgress(notificationId, currentProgress, '正在投圖B面...');
          } else {
            const sideAProgress = Math.floor((currentProgress - 10) / 40 * 100);
            this.notificationService.updateProgress(notificationId, currentProgress, `正在投圖A面... ${sideAProgress}%`);
          }
          break;
          
        case 'sideB':
          currentProgress += 1.5; // B面約25秒完成（50%-90%）
          if (currentProgress >= 90) {
            currentPhase = 'complete';
            this.notificationService.updateProgress(notificationId, currentProgress, '正在完成投圖...');
          } else {
            const sideBProgress = Math.floor((currentProgress - 50) / 40 * 100);
            this.notificationService.updateProgress(notificationId, currentProgress, `正在投圖B面... ${sideBProgress}%`);
          }
          break;
          
        case 'complete':
          currentProgress = Math.min(95, currentProgress + 1);
          this.notificationService.updateProgress(notificationId, currentProgress, '正在完成投圖...');
          break;
      }
    }, 1000); // 每秒更新一次
    
    this.deployService.castImageToDevice(device.id, { cardId: device.currentCardId, side: 2 }).subscribe({
      next: (result: { message: string }) => {
        clearInterval(progressTimer);
        this.notificationService.updateProgress(notificationId, 100, '投圖完成！');
        
        setTimeout(() => {
          this.notificationService.remove(notificationId);
          this.notificationService.success(
            '投圖完成',
            `圖片"${device.currentCardName || '未知圖片'}"投圖已完成！\n圖卡已成功投送到"${device.name}"！`
          );
        }, 1000); // 延長顯示完成狀態
        
        console.log('投圖成功:', result.message);
      },
      error: (error: Error) => {
        clearInterval(progressTimer);
        this.notificationService.remove(notificationId);
        
        // 🎯 根據錯誤類型顯示不同的錯誤訊息
        let errorTitle = '投圖失敗';
        let errorMessage = error.message || '未知錯誤';
        
        if (error.message && error.message.includes('Timeout')) {
          errorTitle = '藍牙連接超時';
          errorMessage = `無法連接到設備 "${device.name}"，請確認：\n1. 設備是否開機並可被發現\n2. 設備是否在藍牙範圍內\n3. 藍牙功能是否正常`;
        } else if (error.message && error.message.includes('Device with address')) {
          errorTitle = '設備連接失敗';
          errorMessage = `找不到設備 "${device.name}"，請重新掃描並連接設備`;
        } else if (error.message && error.message.includes('A面傳輸失敗')) {
          errorTitle = 'A面投圖失敗';
          errorMessage = 'A面圖片傳輸失敗，請檢查設備連接狀態';
        } else if (error.message && error.message.includes('B面傳輸失敗')) {
          errorTitle = 'B面投圖失敗';
          errorMessage = 'B面圖片傳輸失敗，A面可能已成功';
        }
        
        this.notificationService.error(errorTitle, errorMessage);
        console.error('投圖失敗:', error);
      }
    });
  }

  // 投圖到所有設備 - 修改為依序執行
  async castToAllDevices() {
    if (this.devices.length === 0) {
      console.error('沒有可用的設備');
      this.notificationService.warning('無法批量投圖', '沒有可用的設備');
      return;
    }

    const connectedDevices = this.devices.filter(device => 
      device.status === 'Connected' && device.currentCardId
    );
    
    if (connectedDevices.length === 0) {
      this.notificationService.warning('無法批量投圖', '沒有已連接且已部署卡片的設備');
      return;
    }

    console.log(`開始依序投圖到 ${connectedDevices.length} 台設備`);
    
    // 🎯 顯示批量投圖進度，使用與單一設備相同的詳細進度模式
    const notificationId = this.notificationService.loading(
      '正在批量投圖...',
      `準備投圖到 ${connectedDevices.length} 台設備`,
      true
    );
    
    let successfulCasts = 0;
    let failedCasts = 0;

    // 🎯 為每個設備實現詳細的投圖進度追蹤
    for (let i = 0; i < connectedDevices.length; i++) {
      const device = connectedDevices[i];
      const deviceIndex = i + 1;
      
      console.log(`正在投圖到設備 ${deviceIndex}/${connectedDevices.length}:`, { 
        deviceName: device.name, 
        currentCardName: device.currentCardName 
      });
      
      // 🎯 為當前設備建立詳細的進度追蹤系統
      let currentPhase = 'preparing'; // preparing -> sideA -> sideB -> complete
      let currentProgress = 0;
      const baseProgress = (i / connectedDevices.length) * 100; // 當前設備的起始進度
      const deviceProgressRange = 100 / connectedDevices.length; // 每個設備佔用的進度範圍
      
      // 🎯 基於實際投圖時間的進度更新（每個面約25秒，總共約55秒）
      const progressTimer = setInterval(() => {
        switch (currentPhase) {
          case 'preparing':
            currentProgress += 2;
            if (currentProgress >= 10) {
              currentPhase = 'sideA';
              const totalProgress = baseProgress + (currentProgress / 100) * deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(totalProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 正在投圖A面到設備...`
              );
            } else {
              const totalProgress = baseProgress + (currentProgress / 100) * deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(totalProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 準備投圖到設備...`
              );
            }
            break;
            
          case 'sideA':
            currentProgress += 1.5; // A面約25秒完成（10%-50%）
            if (currentProgress >= 50) {
              currentPhase = 'sideB';
              const totalProgress = baseProgress + (currentProgress / 100) * deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(totalProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 正在投圖B面到設備...`
              );
            } else {
              const sideAProgress = Math.floor((currentProgress - 10) / 40 * 100);
              const totalProgress = baseProgress + (currentProgress / 100) * deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(totalProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 投圖A面... ${sideAProgress}%`
              );
            }
            break;
            
          case 'sideB':
            currentProgress += 1.5; // B面約25秒完成（50%-90%）
            if (currentProgress >= 90) {
              currentPhase = 'complete';
              const totalProgress = baseProgress + (currentProgress / 100) * deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(totalProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 正在完成投圖...`
              );
            } else {
              const sideBProgress = Math.floor((currentProgress - 50) / 40 * 100);
              const totalProgress = baseProgress + (currentProgress / 100) * deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(totalProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 投圖B面... ${sideBProgress}%`
              );
            }
            break;
            
          case 'complete': {
            currentProgress = Math.min(95, currentProgress + 1);
            const totalProgress = baseProgress + (currentProgress / 100) * deviceProgressRange;
            this.notificationService.updateProgress(
              notificationId, 
              Math.floor(totalProgress), 
              `設備${deviceIndex}/${connectedDevices.length}: 正在完成投圖...`
            );
            break;
          }
        }
      }, 1000);
      
      try {
        await new Promise<void>((resolve) => {
          this.deployService.castImageToDevice(device.id, { cardId: device.currentCardId!, side: 2 }).subscribe({
            next: (result: { message: string }) => {
              clearInterval(progressTimer);
              // 設備投圖完成，更新進度到該設備的100%
              const completedProgress = baseProgress + deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(completedProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 設備投圖完成！`
              );
              
              console.log('設備投圖成功:', { deviceName: device.name, message: result.message });
              successfulCasts++;
              resolve();
            },
            error: (error: unknown) => {
              clearInterval(progressTimer);
              // 設備投圖失敗，也要更新進度
              const completedProgress = baseProgress + deviceProgressRange;
              this.notificationService.updateProgress(
                notificationId, 
                Math.floor(completedProgress), 
                `設備${deviceIndex}/${connectedDevices.length}: 設備投圖失敗`
              );
              
              console.error('設備投圖失敗:', { deviceName: device.name, error });
              failedCasts++;
              resolve(); // 繼續執行下一個設備
            }
          });
        });
        
        // 每次投圖之間等待 2 秒，避免設備衝突
        if (i < connectedDevices.length - 1) {
          await new Promise(resolve => setTimeout(resolve, 2000));
        }
      } catch (error) {
        clearInterval(progressTimer);
        console.error('設備投圖異常:', { deviceName: device.name, error });
        failedCasts++;
      }
    }

    // 完成進度
    this.notificationService.updateProgress(notificationId, 100, '批量投圖完成！');
    
    setTimeout(() => {
      this.notificationService.remove(notificationId);
      this.finishBatchCast(successfulCasts, failedCasts);
    }, 1000); // 延長顯示完成狀態
  }

  private finishBatchCast(successful: number, failed: number) {
    console.log(`批量投圖完成: ${successful} 成功, ${failed} 失敗`);
    if (failed === 0) {
      this.notificationService.success(
        '批量投圖完成',
        `所有 ${successful} 台設備投圖成功！`
      );
    } else if (successful > 0) {
      this.notificationService.warning(
        '批量投圖完成',
        `${successful} 台成功，${failed} 台失敗`
      );
    } else {
      this.notificationService.error(
        '批量投圖失敗',
        `所有 ${failed} 台設備投圖失敗`
      );
    }
  }

  // 部署卡片到設備
  deployCard(device: Device, card: Card) {
    // 檢查設備是否已經部署了其他卡片
    const existingDeployment = Array.from(this.deployedCards).find(key => 
      key.startsWith(`${device.id}-`) && !key.endsWith(`-${card.id}`)
    );
    
    if (existingDeployment) {
      // 移除設備上的其他部署
      this.deployedCards.delete(existingDeployment);
      console.log(`設備 ${device.name} 移除之前的部署`);
    }
    
    const deployKey = `${device.id}-${card.id}`;
    this.deployedCards.add(deployKey);
    
    this.deployService.deployCardToDevice(device.id, { cardId: card.id, side: 2 }).subscribe({
      next: (result) => {
        console.log('部署成功:', result.message);
        this.loadDevices(); // 重新載入設備列表
      },
      error: (error) => {
        console.error('部署失敗:', error);
        this.deployedCards.delete(deployKey);
      }
    });
  }

  // 檢查卡片是否已部署
  isCardDeployed(device: Device, card: Card): boolean {
    const deployKey = `${device.id}-${card.id}`;
    return this.deployedCards.has(deployKey);
  }

  // 移除設備
  removeDevice(device: Device) {
    this.notificationService.confirm(
      '確認移除設備',
      `確定要移除設備 "${device.name}" 嗎？`,
      () => {
        // 確認回調
        this.deployService.removeDevice(device.id).subscribe({
          next: () => {
            console.log('設備移除成功');
            this.notificationService.success('設備移除成功', `設備 "${device.name}" 已成功移除`);
            this.loadDevices();
          },
          error: (error) => {
            console.error('設備移除失敗:', error);
            this.notificationService.error('設備移除失敗', error.message || '未知錯誤');
          }
        });
      },
      () => {
        // 取消回調
        console.log('取消移除設備');
      }
    );
  }

  // 獲取設備狀態的中文顯示
  getDeviceStatusText(status: string): string {
    switch(status) {
      case 'Connected': return this.text.sections.devices.status.connected;
      case 'Disconnected': return this.text.sections.devices.status.disconnected;
      case 'Error': return this.text.sections.devices.status.error;
      case 'Connecting': return this.text.sections.devices.status.connecting;
      case 'Casting': return this.text.sections.devices.status.casting;
      default: return status;
    }
  }

  // 獲取設備狀態的CSS類
  getDeviceStatusClass(status: string): string {
    switch (status) {
      case 'Connected': return 'status-connected';
      case 'Disconnected': return 'status-disconnected';
      case 'Syncing': return 'status-syncing';
      case 'Error': return 'status-error';
      default: return '';
    }
  }

  // 🛡️ 安全的編號輸入框處理 - 防止 Object Injection
  onDeviceIndexInputBlur(device: Device, event: Event) {
    const target = event.target as HTMLInputElement;
    const value = parseInt(target?.value || '0', 10);
    if (!isNaN(value) && value > 0) {
      // 🛡️ 安全的屬性設置 - 使用直接賦值
      (device as Device & { customIndex?: number }).customIndex = value;
    } else {
      // 🛡️ 安全的屬性刪除
      if (Object.prototype.hasOwnProperty.call(device, 'customIndex')) {
        delete (device as Device & { customIndex?: number }).customIndex;
      }
    }
    this.editIndex = null;
    this.sortDevicesByCustomIndex();
    this.deployService.updateDeviceIndex(device, device.customIndex ?? null).subscribe();
  }

  onIndexArrowClick(device: Device, i: number, delta: number, event: Event) {
    event.stopPropagation();
    this.editIndex = device.bluetoothAddress;
    const current = (device.customIndex ?? (i + 1)) + delta;
    if (current > 0) {
      device.customIndex = current;
    }
    this.sortDevicesByCustomIndex();
    this.deployService.updateDeviceIndex(device, device.customIndex ?? null).subscribe();
    // 讓input自動focus（可選，需用ViewChild或setTimeout）
  }

  // 依 customIndex 排序設備，序號小的在上面
  private sortDevicesByCustomIndex() {
    this.devices.sort((a, b) => {
      const aIndex = a.customIndex ?? (this.devices.indexOf(a) + 1);
      const bIndex = b.customIndex ?? (this.devices.indexOf(b) + 1);
      return aIndex - bIndex;
    });
  }

  // 檢查是否有已連接且已部署卡片的設備
  hasDevicesWithDeployedCards(): boolean {
    return this.devices.some(device => 
      device.status === 'Connected' && device.currentCardId
    );
  }

  openDeployCardModal(device: Device) {
    this.showDeployCardModalFor = device.id;
  }

  closeDeployCardModal() {
    this.showDeployCardModalFor = null;
  }

  // 🛡️ 安全的AB預覽相關方法 - 防止 Object Injection
  toggleCardSide(card: Card, side: 'A' | 'B', event: Event): void {
    event.stopPropagation();
    event.preventDefault();
    
    // 🛡️ 安全的屬性設置 - 使用直接賦值
    (card as Card & { _currentSide?: 'A' | 'B' })._currentSide = side;
  }

  getCurrentSideImage(card: Card): string | undefined {
    // 🛡️ 安全的屬性存取
    const currentSide = this.getCardCurrentSide(card);
    if (currentSide === 'B' && card.thumbnailB) {
      return card.thumbnailB;
    }
    return card.thumbnailA || card.thumbnail;
  }

  getCardCurrentSide(card: Card): 'A' | 'B' {
    // 🛡️ 安全的屬性存取
    if (Object.prototype.hasOwnProperty.call(card, '_currentSide')) {
      const side = (card as Card & { _currentSide?: 'A' | 'B' })._currentSide;
      return (side === 'A' || side === 'B') ? side : 'A';
    }
    return 'A';
  }

  getOtherSidePreview(card: Card): string | undefined {
    const otherSide = this.getCardCurrentSide(card) === 'B' ? 'A' : 'B';
    if (otherSide === 'B' && card.thumbnailB) {
      return card.thumbnailB;
    }
    return card.thumbnailA || card.thumbnail;
  }

  getOtherSideTitle(card: Card): string {
    return this.getCardCurrentSide(card) === 'A' ? 'B面' : 'A面';
  }

  /**
   * 🚀 自動部署 - 將選中的圖卡自動部署到所有已連接的設備
   */
  async autoDeployToAllDevices() {
    if (this.selectedCards.length === 0) {
      this.notificationService.error('請先選擇要部署的圖卡');
      return;
    }

    const connectedDevices = this.devices.filter(device => device.status === 'Connected');
    if (connectedDevices.length === 0) {
      this.notificationService.error('沒有已連接的設備可以部署');
      return;
    }

    // 確認對話框
    const confirmed = confirm(`確定要將 ${this.selectedCards.length} 張圖卡自動部署到 ${connectedDevices.length} 台設備嗎？`);
    if (!confirmed) {
      return;
    }

    let successful = 0;
    let failed = 0;

    this.notificationService.info(`開始自動部署到 ${connectedDevices.length} 台設備...`);

    // 為每個設備分配圖卡（循環分配）
    for (let i = 0; i < connectedDevices.length; i++) {
      const device = connectedDevices[i];
      const cardIndex = i % this.selectedCards.length; // 循環分配圖卡
      const card = this.selectedCards[cardIndex];

      try {
        await this.deployCardToDevice(device, card);
        successful++;
        this.notificationService.success(`${device.name} 部署成功：${card.name}`);
      } catch (error: unknown) {
        failed++;
        console.error(`部署失敗 - 設備: ${device.name}, 圖卡: ${card.name}`, error);
        this.notificationService.error(`${device.name} 部署失敗：${card.name}`);
      }

      // 添加延遲避免過快請求
      await new Promise(resolve => setTimeout(resolve, 500));
    }

    // 完成通知
    if (successful > 0) {
      this.notificationService.success(`自動部署完成！成功: ${successful} 台，失敗: ${failed} 台`);
    } else {
      this.notificationService.error(`自動部署失敗！所有設備都部署失敗`);
    }

    // 重新載入設備狀態
    this.loadDevices();
  }

  /**
   * 🔧 部署圖卡到指定設備（Promise 版本）
   */
  private deployCardToDevice(device: Device, card: Card): Promise<void> {
    return new Promise((resolve, reject) => {
      const deployData = { cardId: card.id, side: 2 }; // 預設部署到B面
      this.deployService.deployCardToDevice(device.id, deployData).subscribe({
        next: () => {
          // 更新本地設備狀態
          device.currentCardId = card.id;
          device.currentCardName = card.name;
          this.deployedCards.add(`${device.id}-${card.id}`);
          resolve();
        },
        error: (error: unknown) => {
          reject(error);
        }
      });
    });
  }

  // 修正 Object Injection Sink 問題 - 使用類型安全的方式
  private setDeviceProperty(device: BluetoothDevice, property: keyof BluetoothDevice, value: unknown): void {
    if (device && typeof device === 'object' && property in device) {
      switch (property) {
        case 'name':
          if (typeof value === 'string') device.name = value;
          break;
        case 'bluetoothAddress':
          if (typeof value === 'string') device.bluetoothAddress = value;
          break;
        case 'originalAddress':
          if (typeof value === 'string' && value !== undefined) device.originalAddress = value;
          break;
        case 'signalStrength':
          if (typeof value === 'number') device.signalStrength = value;
          break;
      }
    }
  }

  private getDeviceProperty(device: BluetoothDevice, property: keyof BluetoothDevice): unknown {
    if (device && typeof device === 'object' && property in device) {
      return device[property];
    }
    return undefined;
  }

  private validateDeviceData(data: unknown): data is BluetoothDevice {
    return data !== null && typeof data === 'object' && 'bluetoothAddress' in (data as object);
  }

  private processDeviceSafely(device: unknown): BluetoothDevice | null {
    if (this.validateDeviceData(device)) {
      return device;
    }
    return null;
  }
} 