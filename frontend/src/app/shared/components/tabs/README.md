# Tab Container 共通組件

這是一個可重用的頁籤容器組件，提供統一的頁籤樣式和內容投射功能。

## 功能特色

- 🎨 統一的頁籤設計風格
- 📱 響應式設計
- 🔧 支援自定義頁籤按鈕
- 📥 使用 `ng-content` 進行內容投射
- ⚡ 支援頁籤切換事件

## 使用方法

### 1. 引入組件

```typescript
import { TabContainerComponent, TabItem } from '../../../shared/components/tabs/tab-container.component';

@Component({
  imports: [TabContainerComponent],
  // ...
})
export class YourComponent {
  // 定義頁籤配置
  tabs: TabItem[] = [
    { key: 'tab1', label: '頁籤一', icon: 'home' },
    { key: 'tab2', label: '頁籤二', icon: 'settings' }
  ];
  
  activeTab = 'tab1';
  
  onTabChange(tabKey: string) {
    this.activeTab = tabKey;
  }
}
```

### 2. 在模板中使用

```html
<sn-tab-container 
  [tabs]="tabs" 
  [activeTab]="activeTab" 
  (tabChange)="onTabChange($event)">
  
  <!-- 頁籤右側按鈕區域 -->
  <div slot="tab-actions">
    <button class="custom-button">自定義按鈕</button>
    <span class="custom-counter">數量：{{ itemCount }}</span>
  </div>

  <!-- 頁籤內容區域 -->
  <div *ngIf="activeTab === 'tab1'">
    <h3>頁籤一內容</h3>
    <p>這裡是第一個頁籤的內容</p>
  </div>

  <div *ngIf="activeTab === 'tab2'">
    <h3>頁籤二內容</h3>
    <p>這裡是第二個頁籤的內容</p>
  </div>

</sn-tab-container>
```

## API

### 輸入屬性 (Inputs)

| 屬性名 | 類型 | 必填 | 說明 |
|--------|------|------|------|
| tabs | TabItem[] | ✅ | 頁籤配置陣列 |
| activeTab | string | ✅ | 目前啟用的頁籤 key |

### 輸出事件 (Outputs)

| 事件名 | 類型 | 說明 |
|--------|------|------|
| tabChange | EventEmitter<string> | 頁籤切換時觸發，回傳選中的頁籤 key |

### TabItem 介面

```typescript
interface TabItem {
  key: string;        // 頁籤唯一識別碼
  label: string;      // 頁籤顯示文字
  icon?: string;      // Material Icon 名稱（選填）
  disabled?: boolean; // 是否停用（選填）
}
```

## 內容投射 (Content Projection)

### tab-actions 插槽

用於放置頁籤右側的按鈕或其他元素：

```html
<div slot="tab-actions">
  <!-- 這裡的內容會顯示在頁籤右側 -->
</div>
```

### 主要內容區域

預設的 `ng-content` 區域，用於放置頁籤內容：

```html
<sn-tab-container>
  <!-- 這裡的內容會顯示在頁籤下方 -->
</sn-tab-container>
```

## 樣式自定義

組件使用 BEM 命名規範：

- `.tab-container` - 主容器
- `.tab-container__tabs` - 頁籤列
- `.tab-container__tabs-left` - 頁籤按鈕區域
- `.tab-container__tabs-right` - 頁籤右側區域
- `.tab-container__content` - 內容區域

可以在父組件的 SCSS 中覆寫樣式：

```scss
::ng-deep .tab-container {
  .tab-container__content {
    padding: 2rem;
  }
}
```

## 完整範例

參考 `deploy.component.ts` 和 `deploy.component.html` 中的使用方式。

## 注意事項

1. 確保每個 `TabItem` 的 `key` 值是唯一的
2. `activeTab` 的值必須對應到某個 `TabItem` 的 `key`
3. 使用 `slot="tab-actions"` 來指定右側按鈕區域
4. 內容區域建議使用 `*ngIf` 來控制顯示/隱藏 