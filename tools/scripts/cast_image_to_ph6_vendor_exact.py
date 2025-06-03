#!/usr/bin/env python3
import asyncio
import logging
import math
import sys
import os
from PIL import Image
from bleak import BleakClient

# 配置參數 - 使用您的設備地址
DEVICE_ADDRESS = "6A422DCC-2730-B0E8-E8B8-1C513A0D7B10"
COMMAND_CHAR_UUID = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E"
ACK_CHAR_UUID = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E"

# 配置日誌
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S"
)

# 配置彩色日誌輸出
logging.addLevelName(
    logging.INFO, "\033[36m%s\033[0m" % logging.getLevelName(logging.INFO))
logging.addLevelName(
    logging.WARNING, "\033[33m%s\033[0m" % logging.getLevelName(logging.WARNING))
logging.addLevelName(
    logging.ERROR, "\033[31m%s\033[0m" % logging.getLevelName(logging.ERROR))
logger = logging.getLogger(__name__)

# 常量定義
DEF_MTU = 247  # 默認BLE MTU值
SEND_PIC_DATA_NO_RES = 2
SEND_PIC_DATA_BLOCK = 3

CRC8_TABLE = [
    0, 94, 188, 226, 97, 63, 221, 131, 194, 156, 126, 32, 163, 253, 31, 65,
    157, 195, 33, 127, 252, 162, 64, 30, 95, 1, 227, 189, 62, 96, 130, 220,
    35, 125, 159, 193, 66, 28, 254, 160, 225, 191, 93, 3, 128, 222, 60, 98,
    190, 224, 2, 92, 223, 129, 99, 61, 124, 34, 192, 158, 29, 67, 161, 255,
    70, 24, 250, 164, 39, 121, 155, 197, 132, 218, 56, 102, 229, 187, 89, 7,
    219, 133, 103, 57, 186, 228, 6, 88, 25, 71, 165, 251, 120, 38, 196, 154,
    101, 59, 217, 135, 4, 90, 184, 230, 167, 249, 27, 69, 198, 152, 122, 36,
    248, 166, 68, 26, 153, 199, 37, 123, 58, 100, 134, 216, 91, 5, 231, 185,
    140, 210, 48, 110, 237, 179, 81, 15, 78, 16, 242, 172, 47, 113, 147, 205,
    17, 79, 173, 243, 112, 46, 204, 146, 211, 141, 111, 49, 178, 236, 14, 80,
    175, 241, 19, 77, 206, 144, 114, 44, 109, 51, 209, 143, 12, 82, 176, 238,
    50, 108, 142, 208, 83, 13, 239, 177, 240, 174, 76, 18, 145, 207, 45, 115,
    202, 148, 118, 40, 171, 245, 23, 73, 8, 86, 180, 234, 105, 55, 213, 139,
    87, 9, 235, 181, 54, 104, 138, 212, 149, 203, 41, 119, 244, 170, 72, 22,
    233, 183, 85, 11, 136, 214, 52, 106, 43, 117, 151, 201, 74, 20, 246, 168,
    116, 42, 200, 150, 21, 75, 169, 247, 182, 232, 10, 84, 215, 137, 107, 53
]

def color_distance(c1, c2):
    dr = c1[0] - c2[0]
    dg = c1[1] - c2[1]
    db = c1[2] - c2[2]
    return math.sqrt(dr * dr + dg * dg + db * db)

def find_nearest_color(target, colors):
    min_distance = color_distance(target, colors[0])
    nearest_index = 0

    for i in range(0, len(colors)):
        distance = color_distance(target, colors[i])
        if distance < min_distance:
            min_distance = distance
            nearest_index = i
    
    # 完全按照廠商的映射
    if nearest_index == 0:
        return 0
    elif nearest_index == 1:
        return 1
    elif nearest_index == 2:
        return 6
    elif nearest_index == 3:
        return 5
    elif nearest_index == 4:
        return 3
    elif nearest_index == 5:
        return 2        
    return nearest_index

def convert_image_to_e6(image_path):
    """完全按照廠商的算法，不做任何修改"""
    buf_size = 192000
    buff = [0]*buf_size  # 保持 list 格式，不轉換為 bytes
    colors = [
        (0, 0, 0),#black
        (255, 255, 255),#white
        (0, 255, 0),#greed
        (0, 0, 255),#blue
        (255, 0, 0),#red
        (255, 255, 0),#yellow
    ]
    rgb_image = Image.open(image_path).convert("RGB")
    
    # 確保圖片尺寸為 800x480
    if rgb_image.size != (800, 480):
        logger.info(f"調整圖片尺寸從 {rgb_image.size} 到 (800, 480)")
        rgb_image = rgb_image.resize((800, 480), Image.Resampling.LANCZOS)
    
    width, height = rgb_image.size
    logger.info(f"處理圖片尺寸: {width}x{height}")
    
    for y in range(height): 
        for x in range(width):
            r, g, b = rgb_image.getpixel((x, y))
            pix = (r,g,b)
            bytePosition = (int)((y*800 + (x))/2)
            bit_mask = 0
            if (x % 2) == 0:
                bit_mask = 4
            else:
                bit_mask = 0
            # bit_mask= 6-bit_mask
            min_distance = find_nearest_color(pix, colors)
            # 完全按照廠商的位運算，不加括號
            buff[bytePosition] = (buff[bytePosition] & (~(3 << bit_mask)) | min_distance << bit_mask)
    
    return buff  # 返回 list，不轉換為 bytes

class BleClient:
    def __init__(self):
        self.ble_connect = False
        self.ble_send_busy = False
        self.ack_event = asyncio.Event()

    def safe_byte(self, value):
        """確保數值在0-255範圍內"""
        return value & 0xFF

    def calculate_crc(self, data):
        """CRC8計算"""
        crc = 0
        for b in data:
            crc = CRC8_TABLE[crc ^ self.safe_byte(b)]
        return crc

    async def send_image_to_ph6(self, client: BleakClient, epd_display_buf, side: int):
        """完全按照廠商的實現"""
        logger.info("send_image_to_ph6")

        is_send_down = 0
        total_pkg_in_block = (32000 // (DEF_MTU - 9 - 3)) + 1
        totalPkg = total_pkg_in_block * 6
        index_in_epd_buf = 0
        currentPkg = 0

        # Step1: 請求寫整面圖片指令
        # 構造初始請求數據包
        data_request = bytearray([0xFE, 0xEF, 0x09, 0x57, 0x01, side, 0xFF, 0xFF, 0xFF])
        data_request[6] = totalPkg & 0xFF
        data_request[7] = (totalPkg >> 8) & 0xFF
        data_request[8] = self.calculate_crc(data_request[:8])
        # 發送初始請求
        await self.ble_send_msg(client, data_request, response=False)

        # 等待響應, 若Step1 CRC error,會有ACK回復。無ACK則繼續Step2發送圖片數據包
        if await self.waitting_for_reply(client, SEND_PIC_DATA_NO_RES, 500):
            logger.info("Total pkg:%d, total_pkg_in_block:%d", totalPkg, total_pkg_in_block)

            # Step2: 發送圖片數據至Device，發送整面數據包
            data_send_pkg = bytearray(DEF_MTU - 3)
            data_send_pkg[0] = 0xFE
            data_send_pkg[1] = 0xEF
            data_send_pkg[3] = 0x57
            data_send_pkg[4] = 0x02

            # 由於桌牌MCU硬件條件限制，只能將整面畫面分成6份進行傳輸，在每1/6中 所有的包中Byte7置為0(表示不需要設備回復)，在每1/6的最後一包需要將Byte7置為1(表示需要設備回復)。
            for j in range(1, 7):  # part循環1-6
                is_send_down = False
                current_pkg_in_block = 0
                while not is_send_down and self.ble_connect:
                    self.ble_send_busy = True
                    
                    # 每1/6的最後一包需要將Byte7置為1(表示需要設備回復)
                    if current_pkg_in_block == (total_pkg_in_block - 1):
                        # 最後一個數據包處理
                        data_size = 32000 - (current_pkg_in_block * (DEF_MTU - 9 - 3)) + 9
                        data_send_pkg[2] = data_size
                        data_send_pkg[5] = currentPkg & 0xFF
                        data_send_pkg[6] = (currentPkg >> 8) & 0xFF
                        data_send_pkg[7] = 0x01  # 需要確認

                        # 複製數據並計算CRC
                        data_send_pkg[8:8 + (data_size - 9)] = \
                            epd_display_buf[index_in_epd_buf : index_in_epd_buf + (data_size - 9)]
                        data_send_pkg[data_size - 1] = \
                            self.calculate_crc(data_send_pkg[:data_size - 1])

                        await self.ble_send_msg(client, data_send_pkg[:data_size], response=True)
                        index_in_epd_buf += (data_size - 9)

                        if not await self.waitting_for_reply(client, SEND_PIC_DATA_BLOCK, 500):
                            await self.close_connection(client)
                        else:
                            logger.info("Current block %d is upload", j)
                        is_send_down = True
                        logger.info("Send pkg_in_block %d %d, side:%d, package_index:%d", 
                                      current_pkg_in_block, data_send_pkg[2], side, j)
                    else:
                        # 常規數據包處理
                        # 每1/6的常規數據包中Byte7置為0(表示不需要設備回復)
                        data_send_pkg[2] = DEF_MTU - 3
                        data_send_pkg[5] = currentPkg & 0xFF
                        data_send_pkg[6] = (currentPkg >> 8) & 0xFF
                        data_send_pkg[7] = 0x00  # 不需要確認

                        # 複製數據並計算CRC
                        chunk_size = DEF_MTU - 3 - 9
                        data_send_pkg[8:8 + chunk_size] = \
                            epd_display_buf[index_in_epd_buf : index_in_epd_buf + chunk_size]
                        data_send_pkg[DEF_MTU - 3 - 1] = \
                            self.calculate_crc(data_send_pkg[:DEF_MTU - 3 - 1])

                        # 完全按照廠商的邏輯
                        if side != 0:
                            if side == 2 and j == 1 and current_pkg_in_block <= 5:
                                await self.ble_send_msg(client, data_send_pkg, response=True)
                            else:
                                await self.ble_send_msg(client, data_send_pkg, response=False)
                        else:
                            await self.ble_send_msg(client, data_send_pkg, response=False)

                        index_in_epd_buf += chunk_size
                        logger.info("Send pkg_in_block %d %d, side:%d, package_index:%d", 
                                      current_pkg_in_block, data_send_pkg[2], side, j)

                    await self.waitting_ble_busy()
                    current_pkg_in_block += 1
                    currentPkg += 1
        else:
            logger.error("send data error") 
        
    async def ble_send_msg(self, client: BleakClient, data: bytes, response: bool):
        await client.write_gatt_char(COMMAND_CHAR_UUID, data, response=response)
        self.ble_send_busy = False

    async def notification_handler(self, sender, data):
        """ACK處理（驗證第6字節）"""
        ack_data = bytes(data)
        if len(ack_data) >= 6 and ack_data[5] == 0x01:
            self.ack_event.set()
            logging.info(f"最後一個數據包ACK: {ack_data.hex()}")
        else:
            logging.info(f"其他數據包ACK: {ack_data.hex()}")
        self.ack_event.clear()

    async def waitting_for_reply(self, client: BleakClient, expected_response: int, timeout: int) -> bool:
        # 完全按照廠商的實現，不加異常處理
        await client.start_notify(ACK_CHAR_UUID, self.notification_handler)
        return True

    async def waitting_ble_busy(self):
        while self.ble_send_busy:
            await asyncio.sleep(0.001)

    async def close_connection(self, client: BleakClient):
        await client.disconnect()
        self.ble_connect = False

async def cast_image_vendor_exact(image_path, side=2):
    """使用完全符合廠商規格的投圖函數"""
    try:
        logger.info(f"開始投送圖片 {image_path} 到設備 {DEVICE_ADDRESS}")
        
        # 轉換圖片
        epd_data = convert_image_to_e6(image_path)
        if len(epd_data) != 192000:
            logger.error(f"無效圖像數據長度: {len(epd_data)}")
            return False
        
        # 連接設備
        client = BleakClient(DEVICE_ADDRESS)
        await client.connect()
        logger.info("成功連接到設備")
        
        # 建立BLE客戶端
        ble = BleClient()
        ble.ble_connect = True
        
        # 發送圖片
        await ble.send_image_to_ph6(client, epd_data, side=side)
        
        # 斷開連接
        await client.disconnect()
        logger.info("設備連接已斷開")
        
        return True
        
    except Exception as e:
        logger.error(f"投送圖片時發生錯誤: {e}")
        return False

def main():
    if len(sys.argv) < 2:
        print("使用方法: python3 cast_image_to_ph6_vendor_exact.py <圖片路徑> [side]")
        print("範例: python3 cast_image_to_ph6_vendor_exact.py test_image.png 2")
        sys.exit(1)
    
    image_path = sys.argv[1]
    side = int(sys.argv[2]) if len(sys.argv) > 2 else 2
    
    # 執行投圖
    success = asyncio.run(cast_image_vendor_exact(image_path, side))
    
    if success:
        print("投圖成功!")
        sys.exit(0)
    else:
        print("投圖失敗!")
        sys.exit(1)

if __name__ == "__main__":
    main() 