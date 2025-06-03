const { Client } = require('pg');
const fs = require('fs');
const path = require('path');

// 資料庫連線設定
const client = new Client({
  host: 'localhost',
  database: 'smart_nameplate',
  username: 'postgres',
  password: 'password',
  port: 5432,
});

async function extractThumbnails() {
  try {
    console.log('🤖 開始從資料庫提取縮圖...');
    
    // 連接資料庫
    await client.connect();
    console.log('✅ 資料庫連接成功');

    // 查詢所有有縮圖的卡片
    const query = `
      SELECT "Id", "Name", "ThumbnailA", "ThumbnailB", "UpdatedAt"
      FROM "Cards" 
      WHERE "ThumbnailA" IS NOT NULL OR "ThumbnailB" IS NOT NULL
      ORDER BY "UpdatedAt" DESC
    `;
    
    const result = await client.query(query);
    console.log(`📋 找到 ${result.rows.length} 張卡片有縮圖資料`);

    if (result.rows.length === 0) {
      console.log('❌ 資料庫中沒有縮圖資料');
      return;
    }

    // 確保thumbnails資料夾存在
    const thumbnailsDir = path.join(__dirname, 'thumbnails');
    if (!fs.existsSync(thumbnailsDir)) {
      fs.mkdirSync(thumbnailsDir, { recursive: true });
    }

    let extractedCount = 0;

    // 處理每張卡片
    for (const card of result.rows) {
      const cardId = card.Id;
      const cardName = card.Name.replace(/[^a-zA-Z0-9\u4e00-\u9fff]/g, '_'); // 清理檔名
      const updateTime = new Date(card.UpdatedAt).toISOString().split('T')[0]; // YYYY-MM-DD格式

      console.log(`\n🏷️ 處理卡片: ${card.Name} (ID: ${cardId})`);

      // 處理A面縮圖
      if (card.ThumbnailA) {
        try {
          const base64Data = card.ThumbnailA.replace(/^data:image\/[a-z]+;base64,/, '');
          const imageBuffer = Buffer.from(base64Data, 'base64');
          const filename = `card_${cardId}_${cardName}_A面_${updateTime}.jpg`;
          const filepath = path.join(thumbnailsDir, filename);
          
          fs.writeFileSync(filepath, imageBuffer);
          console.log(`  📸 A面縮圖已保存: ${filename} (${Math.round(imageBuffer.length / 1024)} KB)`);
          extractedCount++;
        } catch (error) {
          console.error(`  ❌ A面縮圖提取失敗:`, error.message);
        }
      }

      // 處理B面縮圖
      if (card.ThumbnailB) {
        try {
          const base64Data = card.ThumbnailB.replace(/^data:image\/[a-z]+;base64,/, '');
          const imageBuffer = Buffer.from(base64Data, 'base64');
          const filename = `card_${cardId}_${cardName}_B面_${updateTime}.jpg`;
          const filepath = path.join(thumbnailsDir, filename);
          
          fs.writeFileSync(filepath, imageBuffer);
          console.log(`  📸 B面縮圖已保存: ${filename} (${Math.round(imageBuffer.length / 1024)} KB)`);
          extractedCount++;
        } catch (error) {
          console.error(`  ❌ B面縮圖提取失敗:`, error.message);
        }
      }
    }

    console.log(`\n🎉 提取完成！總共保存了 ${extractedCount} 張縮圖到 ./thumbnails/ 資料夾`);
    console.log(`📂 縮圖位置: ${thumbnailsDir}`);

    // 列出所有保存的檔案
    const files = fs.readdirSync(thumbnailsDir).filter(file => file.endsWith('.jpg'));
    console.log('\n📋 已保存的縮圖檔案:');
    files.forEach(file => {
      const filePath = path.join(thumbnailsDir, file);
      const stats = fs.statSync(filePath);
      console.log(`  • ${file} (${Math.round(stats.size / 1024)} KB)`);
    });

  } catch (error) {
    console.error('❌ 提取縮圖時發生錯誤:', error);
  } finally {
    await client.end();
    console.log('🔌 資料庫連接已關閉');
  }
}

// 檢查是否安裝了pg模組
function checkDependencies() {
  try {
    require('pg');
    return true;
  } catch (error) {
    console.log('❌ 缺少 pg 模組，正在安裝...');
    return false;
  }
}

// 主程式
if (require.main === module) {
  if (checkDependencies()) {
    extractThumbnails();
  } else {
    console.log('請先安裝依賴：npm install pg');
    console.log('然後執行：node extract_thumbnails.js');
  }
}

module.exports = { extractThumbnails }; 