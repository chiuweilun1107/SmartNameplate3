#!/usr/bin/env python3
"""
直接使用 render_card_image.py 二進制數據的投圖腳本
避免重複的圖片處理，確保數據一致性
"""
import asyncio
import sys
import requests
from render_card_image import render_card_to_image, convert_to_6_colors_and_binary
from PIL import Image
from cast_image_to_ph6_fixed import BleClientFixed
from bleak import BleakClient
import logging

# 配置日誌
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S"
)
logger = logging.getLogger(__name__)

async def cast_render_direct(card_id, side=2, device_address=None):
    """直接從卡片渲染並投圖，使用統一的數據處理"""
    try:
        logger.info(f"🚀 開始直接渲染投圖: 卡片 {card_id}, 面板: {side}")
        
        # 1. 從API獲取卡片資料
        api_url = f"http://localhost:5001/api/cards/{card_id}"
        response = requests.get(api_url)
        
        if response.status_code != 200:
            logger.error(f"無法取得卡片資料: HTTP {response.status_code}")
            return False
        
        card_data = response.json()
        
        # 檢查是否為 A/B 面相同的卡片
        is_same_both_sides = card_data.get('isSameBothSides', False)
        
        # 根據 side 參數選擇內容
        if side == 0 or (is_same_both_sides and side in [1, 2]):
            # side=0 或 A/B 面相同時，使用 A 面內容
            card_content = card_data.get('contentA')
            side_name = "A面 (A/B面相同)" if is_same_both_sides else "A面 (side=0)"
            final_side = 0  # 強制使用 side=0 進行傳輸
        elif side == 1:
            card_content = card_data.get('contentA')
            side_name = "A面"
            final_side = 1
        elif side == 2:
            card_content = card_data.get('contentB')
            side_name = "B面"
            final_side = 2
        else:
            # 其他值，預設使用 A 面
            card_content = card_data.get('contentA')
            side_name = "A面 (預設)"
            final_side = 1
        
        if not card_content:
            logger.error(f"卡片沒有{side_name}內容資料")
            return False
        
        logger.info(f"📄 準備渲染{side_name}內容")
        
        # 2. 直接渲染為圖片
        logger.info("🎨 開始渲染卡片圖片...")
        temp_image_path = f"temp_card_{card_id}_side_{final_side}_render.png"
        
        if not render_card_to_image(card_content, temp_image_path):
            logger.error("卡片渲染失敗")
            return False
        
        # 3. 載入渲染的圖片並生成二進制數據
        logger.info("🔧 生成E-ink二進制數據...")
        image = Image.open(temp_image_path)
        epd_data = convert_to_6_colors_and_binary(image)
        
        if len(epd_data) != 192000:
            logger.error(f"❌ 無效二進制數據長度: {len(epd_data)}")
            return False
        
        logger.info(f"✅ 二進制數據生成完成: {len(epd_data)} 字節")
        
        # 4. 投圖到設備
        if device_address:
            logger.info(f"📡 開始連接真實設備: {device_address}")
            
            try:
                async with BleakClient(device_address) as client:
                    ble = BleClientFixed()
                    ble.ble_connect = True
                    
                    logger.info("✅ 成功連接到真實設備")
                    
                    # 🔧 關鍵修復：使用test_optimized_parameters.py的成功配置
                    success = await ble.send_image_to_ph6(
                        client, 
                        epd_data, 
                        final_side,  # 使用計算後的 final_side
                        block_delay=0.001,   # 1ms 區塊延遲
                        prep_delay=0.001,    # 1ms 準備延遲  
                        packet_delay=0.001,  # 1ms 包間延遲
                        sync_interval=10     # 🔧 改為10包！解決橫條紋
                    )
                    
                    if success:
                        logger.info("🎉 直接渲染投圖完成！")
                    else:
                        logger.error("❌ 投圖失敗")
                        return False
            except Exception as e:
                logger.error(f"❌ 無法連接到真實設備: {e}")
                return False
        else:
            logger.info("🔄 模擬投圖...")
            # 模擬模式
            await asyncio.sleep(1)
            logger.info("🎉 模擬投圖完成！")
        
        # 5. 清理暫存檔案
        try:
            import os
            if os.path.exists(temp_image_path):
                os.remove(temp_image_path)
                logger.info(f"🧹 已清理暫存檔案: {temp_image_path}")
        except:
            pass
        
        return True
        
    except Exception as e:
        logger.error(f"❌ 直接渲染投圖失敗: {e}")
        return False

def main():
    if len(sys.argv) < 2:
        print("使用方法: python3 cast_render_direct.py <卡片ID> [side] [device_address]")
        print("範例: python3 cast_render_direct.py 1 2 6A422DCC-2730-B0E8-E8B8-1C513A0D7B10")
        sys.exit(1)
    
    card_id = sys.argv[1]
    side = int(sys.argv[2]) if len(sys.argv) > 2 else 2
    device_address = sys.argv[3] if len(sys.argv) > 3 else None
    
    print(f"🎯 直接渲染投圖配置 (橫條紋修復版本):")
    print(f"   📋 卡片ID: {card_id}")
    print(f"   📱 面板: {side}")
    print(f"   🔗 設備: {'真實設備 ' + device_address if device_address else '模擬模式'}")
    print(f"   ⚡ 最佳參數: 1ms延遲 + 10包同步 (無橫條紋)")
    
    # 執行直接渲染投圖
    success = asyncio.run(cast_render_direct(card_id, side, device_address))
    
    if success:
        print("✅ 直接渲染投圖成功!")
        print("🔧 橫條紋解決方案:")
        print("  • 統一的數據處理流程")
        print("  • 廠商相容的二進制格式")
        print("  • 最佳化的傳輸參數 (10包同步)")
        sys.exit(0)
    else:
        print("❌ 直接渲染投圖失敗!")
        sys.exit(1)

if __name__ == "__main__":
    main() 