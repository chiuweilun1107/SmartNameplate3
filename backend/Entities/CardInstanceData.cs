using System.ComponentModel.DataAnnotations;

namespace SmartNameplate.Api.Entities
{
    /// <summary>
    /// 卡片實例資料表
    /// 用於儲存具體填入的內容資料（如：張三、經理、0912345678）
    /// </summary>
    public class CardInstanceData
    {
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// 關聯的卡片ID
        /// </summary>
        [Required]
        public int CardId { get; set; }
        
        /// <summary>
        /// 實例名稱（如：張三的名片、李四的名片）
        /// </summary>
        [Required]
        [StringLength(200)]
        public string InstanceName { get; set; } = string.Empty;
        
        /// <summary>
        /// 面別 (A/B)
        /// </summary>
        [Required]
        [StringLength(1)]
        public string Side { get; set; } = "A";
        
        /// <summary>
        /// 標籤類型
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TagType { get; set; } = string.Empty;
        
        /// <summary>
        /// 實際內容值
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string ContentValue { get; set; } = string.Empty;
        
        /// <summary>
        /// 創建者/使用者ID（未來擴展用）
        /// </summary>
        public int? CreatedBy { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // 導航屬性
        public virtual Card? Card { get; set; }
    }
} 