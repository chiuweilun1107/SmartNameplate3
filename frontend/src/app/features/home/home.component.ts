import { Component, OnInit, OnDestroy, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HomeFeatureCardComponent } from '../../shared/components/cards/card';

@Component({
  selector: 'sn-home',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule,
    HomeFeatureCardComponent
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  @ViewChild('carouselTrack', { static: false }) carouselTrack!: ElementRef;
  
  currentSlide = 1; // 從1開始，因為索引0是克隆的最後一張
  autoplayInterval: any;
  autoplayDelay = 5000; // 5秒自動切換
  isTransitioning = false; // 避免過渡期間的操作
  
  // 原始幻燈片數據
  originalSlides = [
    {
      id: 'main',
      title: 'Magic E-paper',
      subtitle: '現代化的電子紙智慧桌牌管理平台',
      description: '設計、管理與投放，一站式解決方案',
      primaryButton: { text: '聯繫客服', link: '/contact' },
      secondaryButton: { text: '觀看示範', link: '/demo' },
      image: 'assets/images/hero-epaper.png',
      theme: 'default'
    },
    {
      id: 'design',
      title: '強大的設計工具',
      subtitle: '直觀的圖卡設計體驗',
      description: '豐富的模板庫、拖拽式編輯器，讓您輕鬆創建專業桌牌',
      primaryButton: { text: '立即設計', link: '/cards' },
      secondaryButton: { text: '瀏覽模板', link: '/templates' },
      image: 'assets/images/hero-epaper.png',
      theme: 'design'
    },
    {
      id: 'manage',
      title: '智能群組管理',
      subtitle: '高效的批次操作系統',
      description: '統一管理多個桌牌，批次更新內容，節省時間提高效率',
      primaryButton: { text: '管理群組', link: '/groups' },
      secondaryButton: { text: '了解更多', link: '/features' },
      image: 'assets/images/hero-epaper.png',
      theme: 'manage'
    }
  ];

  // 包含克隆幻燈片的完整數組
  get heroSlides() {
    const lastSlide = { ...this.originalSlides[this.originalSlides.length - 1], isClone: true };
    const firstSlide = { ...this.originalSlides[0], isClone: true };
    return [lastSlide, ...this.originalSlides, firstSlide];
  }

  ngOnInit(): void {
    this.startAutoplay();
    // 等待DOM渲染完成後設置初始位置
    setTimeout(() => {
      this.setSlidePosition(1, false);
    }, 0);
  }

  ngOnDestroy(): void {
    this.stopAutoplay();
  }

  nextSlide(): void {
    if (this.isTransitioning) return;
    
    this.isTransitioning = true;
    this.currentSlide++;
    this.setSlidePosition(this.currentSlide, true);
    
    // 如果到達最後一張克隆幻燈片，無動畫跳回第一張真實幻燈片
    if (this.currentSlide === this.heroSlides.length - 1) {
      setTimeout(() => {
        this.currentSlide = 1;
        this.setSlidePosition(1, false);
        this.isTransitioning = false;
      }, 500); // 與CSS過渡時間一致
    } else {
      setTimeout(() => {
        this.isTransitioning = false;
      }, 500);
    }
  }

  previousSlide(): void {
    if (this.isTransitioning) return;
    
    this.isTransitioning = true;
    this.currentSlide--;
    this.setSlidePosition(this.currentSlide, true);
    
    // 如果到達第一張克隆幻燈片，無動畫跳回最後一張真實幻燈片
    if (this.currentSlide === 0) {
      setTimeout(() => {
        this.currentSlide = this.originalSlides.length;
        this.setSlidePosition(this.currentSlide, false);
        this.isTransitioning = false;
      }, 500); // 與CSS過渡時間一致
    } else {
      setTimeout(() => {
        this.isTransitioning = false;
      }, 500);
    }
  }

  goToSlide(index: number): void {
    if (this.isTransitioning) return;
    
    // 指示器點擊的索引是基於原始數組的，需要+1來對應真實位置
    const targetSlide = index + 1;
    if (targetSlide === this.currentSlide) return;
    
    this.isTransitioning = true;
    this.currentSlide = targetSlide;
    this.setSlidePosition(this.currentSlide, true);
    
    setTimeout(() => {
      this.isTransitioning = false;
    }, 500);
  }

  private setSlidePosition(slideIndex: number, withTransition: boolean): void {
    if (this.carouselTrack) {
      const track = this.carouselTrack.nativeElement;
      track.style.transition = withTransition ? 'transform 0.5s cubic-bezier(0.4, 0, 0.2, 1)' : 'none';
      track.style.transform = `translateX(-${slideIndex * 100}%)`;
    }
  }

  startAutoplay(): void {
    this.autoplayInterval = setInterval(() => {
      this.nextSlide();
    }, this.autoplayDelay);
  }

  stopAutoplay(): void {
    if (this.autoplayInterval) {
      clearInterval(this.autoplayInterval);
    }
  }

  onMouseEnter(): void {
    this.stopAutoplay();
  }

  onMouseLeave(): void {
    this.startAutoplay();
  }

  // 獲取當前顯示的真實幻燈片數據（排除克隆）
  get currentSlideData() {
    const realIndex = this.getRealSlideIndex();
    return this.originalSlides[realIndex];
  }

  // 獲取當前幻燈片在原始數組中的索引（用於指示器）
  getRealSlideIndex(): number {
    if (this.currentSlide === 0) {
      return this.originalSlides.length - 1; // 第一張克隆對應最後一張真實
    } else if (this.currentSlide === this.heroSlides.length - 1) {
      return 0; // 最後一張克隆對應第一張真實
    } else {
      return this.currentSlide - 1; // 真實幻燈片索引
    }
  }

  // 檢查指示器是否為活躍狀態
  isIndicatorActive(index: number): boolean {
    return this.getRealSlideIndex() === index;
  }
}