using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities
{
    /// <summary>
    /// 卡片文字元素標籤配置表
    /// 用於儲存每個卡片中文字元素的標籤資訊
    /// </summary>
    public class CardTextElement
    {
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// 關聯的卡片ID
        /// </summary>
        [Required]
        public int CardId { get; set; }
        
        /// <summary>
        /// 面別 (A/B)
        /// </summary>
        [Required]
        [StringLength(1)]
        public string Side { get; set; } = "A";
        
        /// <summary>
        /// 文字元素ID (在JSON content中的element id)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string ElementId { get; set; } = string.Empty;
        
        /// <summary>
        /// 標籤類型 (name, title, phone, address, company, custom)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TagType { get; set; } = string.Empty;
        
        /// <summary>
        /// 標籤顯示名稱
        /// </summary>
        [StringLength(100)]
        public string TagLabel { get; set; } = string.Empty;
        
        /// <summary>
        /// 預設內容
        /// </summary>
        [StringLength(500)]
        public string? DefaultContent { get; set; }
        
        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; } = false;
        
        /// <summary>
        /// 排序序號
        /// </summary>
        public int SortOrder { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // 導航屬性
        public virtual Card? Card { get; set; }
    }
} 