const { Client } = require('pg');

// 資料庫連線設定
const client = new Client({
  host: 'localhost',
  database: 'smart_nameplate',
  username: 'postgres',
  password: 'password',
  port: 5432,
});

async function fixBoundaryElements() {
  try {
    console.log('🤖 開始修正超出邊界的元素...');
    
    await client.connect();
    console.log('✅ 資料庫連接成功');

    // 查詢所有卡片
    const query = 'SELECT "Id", "Name", "ContentA", "ContentB" FROM "Cards" ORDER BY "UpdatedAt" DESC';
    const result = await client.query(query);
    
    console.log(`📋 找到 ${result.rows.length} 張卡片`);
    
    let fixedCount = 0;
    
    for (const card of result.rows) {
      let hasChanges = false;
      let contentA = card.ContentA;
      let contentB = card.ContentB;
      
      // 修正A面
      if (contentA && contentA.elements) {
        const canvasWidth = contentA.width || 800;
        const canvasHeight = contentA.height || 480;
        
        contentA.elements.forEach(element => {
          const originalX = element.position.x;
          const originalY = element.position.y;
          const originalWidth = element.size.width;
          const originalHeight = element.size.height;
          
          // 檢查右邊界
          if (element.position.x + element.size.width > canvasWidth) {
            element.position.x = Math.max(0, canvasWidth - element.size.width);
            console.log(`  📐 修正元素 ${element.id} X位置: ${originalX} → ${element.position.x}`);
            hasChanges = true;
          }
          
          // 檢查下邊界
          if (element.position.y + element.size.height > canvasHeight) {
            element.position.y = Math.max(0, canvasHeight - element.size.height);
            console.log(`  📐 修正元素 ${element.id} Y位置: ${originalY} → ${element.position.y}`);
            hasChanges = true;
          }
          
          // 檢查左邊界
          if (element.position.x < 0) {
            element.position.x = 0;
            console.log(`  📐 修正元素 ${element.id} X位置: ${originalX} → 0`);
            hasChanges = true;
          }
          
          // 檢查上邊界
          if (element.position.y < 0) {
            element.position.y = 0;
            console.log(`  📐 修正元素 ${element.id} Y位置: ${originalY} → 0`);
            hasChanges = true;
          }
        });
      }
      
      // 修正B面
      if (contentB && contentB.elements) {
        const canvasWidth = contentB.width || 800;
        const canvasHeight = contentB.height || 480;
        
        contentB.elements.forEach(element => {
          const originalX = element.position.x;
          const originalY = element.position.y;
          
          // 檢查右邊界
          if (element.position.x + element.size.width > canvasWidth) {
            element.position.x = Math.max(0, canvasWidth - element.size.width);
            console.log(`  📐 修正B面元素 ${element.id} X位置: ${originalX} → ${element.position.x}`);
            hasChanges = true;
          }
          
          // 檢查下邊界
          if (element.position.y + element.size.height > canvasHeight) {
            element.position.y = Math.max(0, canvasHeight - element.size.height);
            console.log(`  📐 修正B面元素 ${element.id} Y位置: ${originalY} → ${element.position.y}`);
            hasChanges = true;
          }
          
          // 檢查左邊界
          if (element.position.x < 0) {
            element.position.x = 0;
            console.log(`  📐 修正B面元素 ${element.id} X位置: ${originalX} → 0`);
            hasChanges = true;
          }
          
          // 檢查上邊界
          if (element.position.y < 0) {
            element.position.y = 0;
            console.log(`  📐 修正B面元素 ${element.id} Y位置: ${originalY} → 0`);
            hasChanges = true;
          }
        });
      }
      
      // 如果有修改，更新資料庫
      if (hasChanges) {
        console.log(`🔧 更新卡片: ${card.Name} (ID: ${card.Id})`);
        
        const updateQuery = `
          UPDATE "Cards" 
          SET "ContentA" = $1, "ContentB" = $2, "UpdatedAt" = CURRENT_TIMESTAMP
          WHERE "Id" = $3
        `;
        
        await client.query(updateQuery, [JSON.stringify(contentA), JSON.stringify(contentB), card.Id]);
        fixedCount++;
      }
    }
    
    console.log(`\n🎉 修正完成！總共修正了 ${fixedCount} 張卡片`);
    
  } catch (error) {
    console.error('❌ 修正過程中發生錯誤:', error);
  } finally {
    await client.end();
    console.log('🔌 資料庫連接已關閉');
  }
}

// 主程式
if (require.main === module) {
  fixBoundaryElements();
}

module.exports = { fixBoundaryElements }; 