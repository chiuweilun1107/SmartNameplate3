# 🛠️ SmartNameplate 開發工具

## 📁 資料夾結構

```
tools/
├── scripts/           # 開發腳本
│   ├── *.py          # Python 腳本
│   └── *.js          # JavaScript 腳本
├── data/             # 測試資料
│   ├── *.bin         # 二進位測試檔案
│   └── *.txt         # 文字測試資料
├── package.json      # Node.js 依賴
└── README.md         # 本說明文件
```

## 🚀 使用方式

### 安裝依賴
```bash
cd tools
npm install
```

### 執行腳本
```bash
# 提取縮略圖
npm run extract-thumbnails

# 修復邊界元素
npm run fix-boundary

# 或直接執行
node scripts/extract_thumbnails.js
node scripts/fix_boundary_elements.js
```

### Python 腳本
```bash
# 執行 Python 腳本
python scripts/render_card_image.py
python scripts/cast_image_to_ph6.py
```

## 📋 腳本說明

### JavaScript 腳本
- `extract_thumbnails.js` - 從資料庫提取縮略圖
- `fix_boundary_elements.js` - 修復邊界元素

### Python 腳本
- `render_card_image.py` - 渲染卡片圖片
- `cast_image_to_ph6.py` - 轉換圖片到 E-ink 格式
- `backend_ble_scanner.py` - 藍牙掃描工具

### 測試資料
- `our_algorithm_data.bin` - 演算法測試資料
- `thumbnail_data.txt` - 縮略圖測試資料 