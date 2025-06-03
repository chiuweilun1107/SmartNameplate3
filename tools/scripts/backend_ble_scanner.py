#!/usr/bin/env python3
import asyncio
import json
import sys
from bleak import BleakScanner, BleakClient

# PH6 桌牌的 UUID
PH6_SERVICE_UUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E"
PH6_COMMAND_UUID = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E"
PH6_ACK_UUID = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E"

def extract_real_mac_from_advertisement(device, advertisement_data):
    """嘗試從廣告數據中提取真實的 MAC 地址"""
    try:
        # PH6 設備的真實 MAC 地址存儲在特定的服務數據中
        target_service_uuid = "00002001-0000-1000-8000-00805f9b34fb"
        
        # 檢查服務數據
        if advertisement_data.service_data:
            for uuid, data in advertisement_data.service_data.items():
                # 檢查是否是包含 MAC 地址的服務
                if str(uuid).lower() == target_service_uuid.lower():
                    if len(data) == 6:  # MAC 地址恰好是 6 bytes
                        # 反轉字節序以獲得正確的 MAC 地址
                        reversed_data = data[::-1]
                        mac_address = ':'.join([f'{b:02X}' for b in reversed_data])
                        return mac_address
        
        # 如果沒有找到特定服務的 MAC 地址，嘗試其他服務數據
        if advertisement_data.service_data:
            for uuid, data in advertisement_data.service_data.items():
                if len(data) == 6:  # 尋找 6 bytes 的數據
                    # 檢查是否看起來像有效的 MAC 地址（不是全零或全 FF）
                    if not all(b == 0 for b in data) and not all(b == 0xFF for b in data):
                        # 反轉字節序
                        reversed_data = data[::-1]
                        mac_address = ':'.join([f'{b:02X}' for b in reversed_data])
                        return mac_address
        
        # 檢查製造商數據（備選方案）
        if advertisement_data.manufacturer_data:
            for company_id, data in advertisement_data.manufacturer_data.items():
                if len(data) >= 6:
                    # 嘗試不同位置的 6 bytes 組合
                    for offset in range(len(data) - 5):
                        mac_bytes = data[offset:offset+6]
                        # 檢查是否看起來像有效的 MAC 地址
                        if not all(b == 0 for b in mac_bytes) and not all(b == 0xFF for b in mac_bytes):
                            # 反轉字節序
                            reversed_bytes = mac_bytes[::-1]
                            mac_address = ':'.join([f'{b:02X}' for b in reversed_bytes])
                            return mac_address
        
        # 如果沒有找到，返回 None
        return None
        
    except Exception as e:
        return None

def is_likely_nameplate(name, address):
    """判斷是否可能是桌牌設備"""
    if not name:
        return False
    
    name_lower = name.lower()
    
    # PH6 桌牌的識別模式
    nameplate_patterns = [
        "ph6", "nameplate", "eink", "epd", "epaper",
        "smart", "display"
    ]
    
    for pattern in nameplate_patterns:
        if pattern in name_lower:
            return True
    
    # 檢查是否以特定模式開頭
    if len(name) > 3:
        if name.lower().startswith(('a1', 'hpa', 'ph')):
            return True
    
    return False

async def verify_ph6_device(device):
    """驗證設備是否為真正的 PH6 桌牌"""
    try:
        async with BleakClient(device.address, timeout=10.0) as client:
            # 嘗試連接並檢查服務
            if not client.is_connected:
                return False
                
            services = client.services
            
            # 檢查是否有 PH6 的服務和特徵值
            has_ph6_service = False
            has_command_char = False
            has_ack_char = False
            
            for service in services:
                if str(service.uuid).upper() == PH6_SERVICE_UUID.upper():
                    has_ph6_service = True
                    
                    for char in service.characteristics:
                        char_uuid = str(char.uuid).upper()
                        if char_uuid == PH6_COMMAND_UUID.upper():
                            has_command_char = True
                        elif char_uuid == PH6_ACK_UUID.upper():
                            has_ack_char = True
            
            return has_ph6_service and has_command_char and has_ack_char
            
    except Exception:
        return False

async def scan_for_nameplates():
    """掃描桌牌設備"""
    try:
        discovered_devices = []
        
        def detection_callback(device, advertisement_data):
            device_name = advertisement_data.local_name or device.name or "Unknown Device"
            device_address = device.address
            
            # 獲取信號強度
            rssi = advertisement_data.rssi
            
            # 檢查是否可能是桌牌設備
            if is_likely_nameplate(device_name, device_address):
                # 嘗試獲取真實的 MAC 地址
                real_mac = extract_real_mac_from_advertisement(device, advertisement_data)
                final_address = real_mac if real_mac else device_address
                
                device_info = {
                    "Name": device_name,
                    "BluetoothAddress": final_address,  # 格式化的 MAC 地址，用於顯示
                    "OriginalAddress": device_address,  # 原始的 UUID 地址，用於 BLE 連接
                    "RealMacFound": real_mac is not None,
                    "SignalStrength": rssi,
                    "IsConnected": False,
                    "DeviceType": "Smart Nameplate"
                }
                
                # 避免重複添加
                if not any(d["OriginalAddress"] == device_address for d in discovered_devices):
                    discovered_devices.append(device_info)
        
        # 使用回調函數進行掃描
        scanner = BleakScanner(detection_callback)
        await scanner.start()
        await asyncio.sleep(10.0)  # 掃描 10 秒
        await scanner.stop()
        
        # 暫時跳過 PH6 驗證，直接返回發現的設備
        return discovered_devices
        
    except Exception as e:
        return []

async def main():
    """主函數"""
    try:
        devices = await scan_for_nameplates()
        # 輸出 JSON 格式的結果供 .NET 解析
        print(json.dumps(devices, ensure_ascii=False, indent=2))
    except Exception as e:
        # 輸出空陣列表示失敗
        print("[]")

if __name__ == "__main__":
    asyncio.run(main()) 