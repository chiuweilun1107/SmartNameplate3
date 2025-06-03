#!/usr/bin/env python3
import json
import sys
import os
import math
from PIL import Image, ImageDraw, ImageFont
import qrcode
import requests
import io

def render_card_to_image(card_content, output_path="temp_card.png", width=800, height=480):
    """
    從卡片JSON內容渲染成圖片，支援6色轉換
    """
    try:
        # 解析卡片內容
        if isinstance(card_content, str):
            content = json.loads(card_content)
        else:
            content = card_content
        
        # 支援新的資料結構：直接傳入面的內容，或舊的包含 'A' 鍵的結構
        if 'A' in content:
            # 舊的資料結構
            side_a = content['A']
        elif 'elements' in content:
            # 新的資料結構：直接傳入面的內容
            side_a = content
        else:
            print("錯誤：卡片內容格式不正確，缺少元素資料")
            return False
        
        # 建立圖片畫布 - 固定為桌牌尺寸 800x480
        canvas_width = 800
        canvas_height = 480
        background_color = side_a.get('background', '#ffffff')
        
        # 建立新圖片
        image = Image.new('RGB', (canvas_width, canvas_height), background_color)
        draw = ImageDraw.Draw(image)
        
        # 依z-index排序元素
        elements = side_a.get('elements', [])
        elements.sort(key=lambda x: x.get('zIndex', 0))
        
        # 渲染每個元素
        for element in elements:
            render_element(image, draw, element)
        
        # 將圖片轉換為6色格式
        image_6color = convert_to_6_colors(image)
        
        # 儲存圖片
        image_6color.save(output_path, 'PNG')
        print(f"圖片已儲存至: {output_path} (800x480, 6色)")
        return True
        
    except Exception as e:
        print(f"渲染圖片時發生錯誤: {e}")
        import traceback
        traceback.print_exc()
        return False

def convert_to_6_colors_and_binary(image):
    """將圖片轉換為6色格式並生成E-ink二進制數據 - 完全模仿廠商算法"""
    # 定義6色調色盤 - 與廠商算法完全一致
    colors = [
        (0, 0, 0),          # 0: 黑色
        (255, 255, 255),    # 1: 白色
        (0, 255, 0),        # 2: 綠色
        (0, 0, 255),        # 3: 藍色
        (255, 0, 0),        # 4: 紅色
        (255, 255, 0),      # 5: 黃色
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
        
        # 廠商映射 - 關鍵！
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
    
    # 關鍵修復：生成二進制數據而不是圖片
    buf_size = 192000
    buff = [0] * buf_size  # 保持 list 格式，與廠商一致
    
    # 確保圖片尺寸正確
    if image.size != (800, 480):
        image = image.resize((800, 480), Image.Resampling.LANCZOS)
    
    width, height = image.size
    print(f"🎨 開始二進制轉換: {width}x{height} 像素")
    print("🔧 使用廠商相容的映射邏輯")
    
    # 完全按照廠商的二進制封裝算法
    for y in range(height): 
        for x in range(width):
            r, g, b = image.getpixel((x, y))
            pix = (r, g, b)
            bytePosition = int((y * 800 + x) / 2)
            bit_mask = 0
            if (x % 2) == 0:
                bit_mask = 4
            else:
                bit_mask = 0
            
            min_distance = find_nearest_color(pix, colors)
            # 廠商的位運算 - 關鍵！
            buff[bytePosition] = (buff[bytePosition] & (~(3 << bit_mask)) | min_distance << bit_mask)
    
    print("✅ 二進制轉換完成，已套用廠商映射和位運算")
    return buff

def convert_to_6_colors(image):
    """保留原有的圖片轉換功能，用於顯示預覽"""
    # 定義6色調色盤 - 與廠商算法完全一致
    colors = [
        (0, 0, 0),          # 0: 黑色
        (255, 255, 255),    # 1: 白色
        (0, 255, 0),        # 2: 綠色
        (0, 0, 255),        # 3: 藍色
        (255, 0, 0),        # 4: 紅色
        (255, 255, 0),      # 5: 黃色
    ]
    
    def color_distance(c1, c2):
        dr = c1[0] - c2[0]
        dg = c1[1] - c2[1]
        db = c1[2] - c2[2]
        return math.sqrt(dr * dr + dg * dg + db * db)
    
    def find_nearest_color(target):
        min_distance = color_distance(target, colors[0])
        nearest_index = 0

        for i in range(len(colors)):
            distance = color_distance(target, colors[i])
            if distance < min_distance:
                min_distance = distance
                nearest_index = i
        
        # 關鍵修復：廠商映射邏輯
        if nearest_index == 0:
            return 0, colors[0]  # 黑色
        elif nearest_index == 1:
            return 1, colors[1]  # 白色
        elif nearest_index == 2:
            return 6, colors[2]  # 綠色映射到6
        elif nearest_index == 3:
            return 5, colors[3]  # 藍色映射到5
        elif nearest_index == 4:
            return 3, colors[4]  # 紅色映射到3
        elif nearest_index == 5:
            return 2, colors[5]  # 黃色映射到2        
        return nearest_index, colors[nearest_index]
    
    # 轉換每個像素
    converted_image = Image.new('RGB', image.size)
    width, height = image.size
    
    print(f"🎨 開始6色轉換: {width}x{height} 像素")
    print("🔧 使用廠商相容的顏色映射邏輯")
    
    for y in range(height):
        for x in range(width):
            original_color = image.getpixel((x, y))
            mapped_index, new_color = find_nearest_color(original_color)
            converted_image.putpixel((x, y), new_color)
    
    print("✅ 6色轉換完成，已套用廠商映射")
    return converted_image

def render_element(image, draw, element):
    """渲染單個元素"""
    element_type = element.get('type', 'unknown')
    position = element.get('position', {'x': 0, 'y': 0})
    size = element.get('size', {'width': 100, 'height': 100})
    style = element.get('style', {})
    
    x = position['x']
    y = position['y']
    width = size['width']
    height = size['height']
    
    # 確保座標在畫布範圍內
    x = max(0, min(x, 800 - width))
    y = max(0, min(y, 480 - height))
    
    if element_type == 'text':
        render_text_element(image, draw, element, x, y, width, height)
    elif element_type == 'qrcode':
        render_qrcode_element(image, element, x, y, width, height)
    elif element_type == 'image':
        render_image_element(image, element, x, y, width, height)
    elif element_type == 'rectangle':
        render_rectangle_element(image, draw, element, x, y, width, height)

def render_text_element(image, draw, element, x, y, width, height):
    """渲染文字元素，支援中文字體"""
    try:
        content = element.get('content', 'Text')
        style = element.get('style', {})
        
        # 字型設定
        font_family = style.get('fontFamily', 'Arial')
        font_size = style.get('fontSize', 16)
        font_weight = style.get('fontWeight', 'normal')
        color = style.get('color', '#000000')
        text_align = style.get('textAlign', 'left')
        
        # 嘗試載入字型，優先支援中文
        font = get_font(font_family, font_size, font_weight)
        
        # 背景色
        bg_color = style.get('backgroundColor', 'transparent')
        if bg_color != 'transparent':
            draw.rectangle([x, y, x + width, y + height], fill=bg_color)
        
        # 計算文字位置
        bbox = draw.textbbox((0, 0), content, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]
        
        if text_align == 'center':
            text_x = x + (width - text_width) // 2
        elif text_align == 'right':
            text_x = x + width - text_width
        else:
            text_x = x
            
        text_y = y + (height - text_height) // 2
        
        # 繪製文字
        draw.text((text_x, text_y), content, fill=color, font=font)
        
    except Exception as e:
        print(f"渲染文字元素時發生錯誤: {e}")

def get_font(font_family, font_size, font_weight):
    """獲取字體，優先支援中文，增強字體回退機制"""
    font_paths = []
    
    # macOS 中文字體路徑（更全面的字體清單）
    chinese_fonts = [
        '/System/Library/Fonts/PingFang.ttc',
        '/System/Library/Fonts/Supplemental/Songti.ttc',
        '/System/Library/Fonts/Supplemental/STHeiti Light.ttc',
        '/System/Library/Fonts/Supplemental/STHeiti Medium.ttc',
        '/Library/Fonts/Microsoft JhengHei.ttf',
        '/System/Library/Fonts/Helvetica.ttc',
        '/System/Library/Fonts/Arial.ttf',
        '/System/Library/Fonts/Geneva.ttf',
        '/Library/Fonts/Arial Unicode MS.ttf',
        '/System/Library/Fonts/Apple Symbols.ttf',
        # 新增更多中文字體
        '/System/Library/Fonts/Hiragino Sans GB.ttc',
        '/System/Library/Fonts/STIXGeneral.otf',
        '/Library/Fonts/SimHei.ttf',
        '/Library/Fonts/SimSun.ttf'
    ]
    
    # 根據字體家族選擇，優先使用中文字體
    if any(keyword in font_family.lower() for keyword in ['微軟正黑體', 'microsoft', 'jhenghei', '蘋方', 'pingfang', '宋體', 'songti', '黑體', 'heiti']):
        # 中文字體優先
        font_paths.extend(chinese_fonts)
    else:
        # 英文字體但仍添加中文字體作為備選
        font_paths.extend([
            f'/System/Library/Fonts/{font_family}.ttf',
            f'/System/Library/Fonts/{font_family}.ttc',
        ])
        font_paths.extend(chinese_fonts)  # 作為備選
    
    # 嘗試載入字體
    for font_path in font_paths:
        try:
            if os.path.exists(font_path):
                font = ImageFont.truetype(font_path, font_size)
                print(f"成功載入字體: {font_path}")
                return font
        except Exception as e:
            print(f"嘗試載入字體失敗 {font_path}: {e}")
            continue
    
    # 如果所有字體都失敗，嘗試系統默認字體
    print("所有指定字體都無法載入，嘗試系統默認字體")
    try:
        # 嘗試載入系統默認中文字體
        default_font = ImageFont.load_default()
        print("使用系統默認字體")
        return default_font
    except Exception as e:
        print(f"載入默認字體也失敗: {e}")
        # 最後的回退
        return ImageFont.load_default()

def render_qrcode_element(image, element, x, y, width, height):
    """渲染QR碼元素"""
    try:
        data = element.get('data', 'https://example.com')
        style = element.get('style', {})
        
        if not data.strip():
            data = 'https://example.com'
        
        # QR碼設定
        qr = qrcode.QRCode(
            version=1,
            error_correction=qrcode.constants.ERROR_CORRECT_L,
            box_size=10,
            border=1,
        )
        qr.add_data(data)
        qr.make(fit=True)
        
        # 建立QR碼圖片
        fg_color = style.get('foregroundColor', '#000000')
        bg_color = style.get('backgroundColor', '#ffffff')
        
        qr_img = qr.make_image(fill_color=fg_color, back_color=bg_color)
        qr_img = qr_img.resize((width, height), Image.Resampling.LANCZOS)
        
        # 貼到主圖片上
        image.paste(qr_img, (x, y))
        
    except Exception as e:
        print(f"渲染QR碼元素時發生錯誤: {e}")

def render_image_element(image, element, x, y, width, height):
    """渲染圖片元素"""
    try:
        src = element.get('src', '')
        if not src:
            # 繪製佔位符
            draw = ImageDraw.Draw(image)
            draw.rectangle([x, y, x + width, y + height], outline='#cccccc', fill='#f5f5f5')
            draw.text((x + 10, y + 10), 'Image', fill='#666666')
            return
        
        # 載入圖片（可以是本地檔案或URL）
        if src.startswith('http'):
            response = requests.get(src)
            img = Image.open(io.BytesIO(response.content))
        else:
            img = Image.open(src)
        
        # 調整尺寸
        img = img.resize((width, height), Image.Resampling.LANCZOS)
        
        # 貼到主圖片上
        image.paste(img, (x, y))
        
    except Exception as e:
        print(f"渲染圖片元素時發生錯誤: {e}")

def render_rectangle_element(image, draw, element, x, y, width, height):
    """渲染矩形元素"""
    try:
        style = element.get('style', {})
        
        # 背景色
        bg_color = style.get('backgroundColor', '#cccccc')
        border_color = style.get('borderColor', '#000000')
        border_width = style.get('borderWidth', 1)
        
        # 繪製矩形
        if bg_color != 'transparent':
            draw.rectangle([x, y, x + width, y + height], fill=bg_color)
        
        if border_width > 0:
            draw.rectangle([x, y, x + width, y + height], outline=border_color, width=border_width)
            
    except Exception as e:
        print(f"渲染矩形元素時發生錯誤: {e}")

def main():
    if len(sys.argv) < 2:
        print("使用方法: python3 render_card_image.py <卡片ID> [輸出檔案名] [面(A/B)]")
        print("範例: python3 render_card_image.py 6 rendered_card.png A")
        print("      python3 render_card_image.py 6 rendered_card.png B")
        sys.exit(1)
    
    card_id = sys.argv[1]
    output_file = sys.argv[2] if len(sys.argv) > 2 else f"card_{card_id}.png"
    side = sys.argv[3] if len(sys.argv) > 3 else ""  # A、B 或空字串
    
    try:
        # 從後端API取得卡片資料
        api_url = f"http://localhost:5001/api/cards/{card_id}"
        response = requests.get(api_url)
        
        if response.status_code != 200:
            print(f"無法取得卡片資料: HTTP {response.status_code}")
            sys.exit(1)
        
        card_data = response.json()
        
        # 根據側面參數決定使用哪個內容
        if side.upper() == "B":
            # 指定B面
            card_content = card_data.get('contentB')
            if not card_content:
                print(f"❌ 卡片 {card_id} 沒有B面內容資料 (contentB)")
                sys.exit(1)
            print(f"🎨 渲染卡片 {card_id} B面內容")
        elif side.upper() == "A":
            # 指定A面
            card_content = card_data.get('contentA')
            if not card_content:
                print(f"❌ 卡片 {card_id} 沒有A面內容資料 (contentA)")
                sys.exit(1)
            print(f"🎨 渲染卡片 {card_id} A面內容")
        else:
            # 預設行為：優先使用 contentA，如果沒有則嘗試舊的 content
            card_content = card_data.get('contentA') or card_data.get('content')
            if not card_content:
                print("❌ 卡片沒有內容資料 (檢查 contentA 和 content 欄位)")
                print(f"可用欄位: {list(card_data.keys())}")
                sys.exit(1)
            print(f"🎨 渲染卡片 {card_id} 預設內容 (A面或舊格式)")
        
        # 渲染圖片
        success = render_card_to_image(card_content, output_file)
        
        if success:
            print(f"✅ 卡片 {card_id} 渲染成功: {output_file}")
            sys.exit(0)
        else:
            print("❌ 渲染失敗")
            sys.exit(1)
            
    except Exception as e:
        print(f"程式執行錯誤: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)

if __name__ == "__main__":
    main() 