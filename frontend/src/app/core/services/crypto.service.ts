import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class CryptoService {

  /**
   * 🛡️ 安全的隨機字符串生成器
   * 使用 Web Crypto API 替代不安全的 Math.random()
   */
  generateSecureRandomString(length = 9): string {
    const chars = 'abcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    
    if (typeof window !== 'undefined' && window.crypto && window.crypto.getRandomValues) {
      // 使用安全的 Web Crypto API
      const randomValues = new Uint8Array(length);
      window.crypto.getRandomValues(randomValues);
      
      for (let i = 0; i < length; i++) {
        const randomValue = randomValues.at(i) || 0;
        const index = randomValue % chars.length;
        result += chars.charAt(index);
      }
    } else {
      // 後備方案：使用 Date.now() + counter 的組合
      const timestamp = Date.now().toString(36);
      const counter = this.getCounter().toString(36);
      result = timestamp + counter;
      result = result.substring(0, length);
    }
    
    return result;
  }

  /**
   * 🛡️ 生成安全的元素ID
   */
  generateElementId(): string {
    return 'el_' + this.generateSecureRandomString(9);
  }

  /**
   * 🛡️ 生成安全的設計ID  
   */
  generateDesignId(): string {
    const timestamp = Date.now();
    const randomPart = this.generateSecureRandomString(9);
    return `new_${timestamp}_${randomPart}`;
  }

  /**
   * 🛡️ 生成安全的用戶ID
   */
  generateUserId(): string {
    return 'user_' + this.generateSecureRandomString(9);
  }

  /**
   * 🛡️ 生成安全的通知ID
   */
  generateNotificationId(): string {
    return this.generateSecureRandomString(12);
  }

  /**
   * 🛡️ 安全的顏色選擇（用於協作）
   */
  selectSecureRandomColor(colors: string[]): string {
    if (colors.length === 0) return '#000000';
    
    if (typeof window !== 'undefined' && window.crypto && window.crypto.getRandomValues) {
      const randomArray = new Uint8Array(1);
      window.crypto.getRandomValues(randomArray);
      const index = randomArray[0] % colors.length;
      return colors.at(index) || colors[0];
    } else {
      // 後備方案：使用時間戳作為種子
      const index = Date.now() % colors.length;
      return colors.at(index) || colors[0];
    }
  }

  /**
   * 🛡️ 安全的進度增量（用於部署進度）
   */
  generateSecureProgressIncrement(min = 5, max = 20): number {
    if (typeof window !== 'undefined' && window.crypto && window.crypto.getRandomValues) {
      const randomArray = new Uint8Array(1);
      window.crypto.getRandomValues(randomArray);
      const range = max - min;
      return min + (randomArray[0] % range);
    } else {
      // 後備方案：使用固定增量
      return Math.floor((min + max) / 2);
    }
  }

  /**
   * 🛡️ 安全的布林值生成（用於QR Code圖案）
   */
  generateSecureBoolean(): boolean {
    if (typeof window !== 'undefined' && window.crypto && window.crypto.getRandomValues) {
      const randomArray = new Uint8Array(1);
      window.crypto.getRandomValues(randomArray);
      return randomArray[0] % 2 === 0;
    } else {
      // 後備方案：使用時間戳的奇偶性
      return Date.now() % 2 === 0;
    }
  }

  private counter = 0;
  
  private getCounter(): number {
    this.counter = (this.counter + 1) % 1000000;
    return this.counter;
  }
} 