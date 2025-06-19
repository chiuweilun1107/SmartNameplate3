#!/usr/bin/env python3
import sys
import os
import json

print("🐍 Python 腳本執行成功！")
print(f"📂 當前工作目錄: {os.getcwd()}")
print(f"📋 Python 版本: {sys.version}")
print(f"🔢 參數數量: {len(sys.argv)}")
print(f"📝 參數內容: {sys.argv}")

# 測試 JSON 輸出
result = {
    "success": True,
    "message": "Python 腳本測試成功",
    "working_directory": os.getcwd(),
    "python_version": sys.version,
    "arguments": sys.argv
}

print(json.dumps(result, ensure_ascii=False, indent=2)) 