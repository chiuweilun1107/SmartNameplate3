using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Domain.Shared.Dto.Backend.Common;

/// <summary>
/// 🤖 業務邏輯回應基類
/// 所有業務操作回應的基礎類別
/// </summary>
public class BusinessLogicResponse
{
    #region Properties
    
    /// <summary>
    /// 是否成功
    /// </summary>
    [JsonProperty("success")]
    public bool Success { get; set; } = true;

    /// <summary>
    /// 訊息
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    [JsonProperty("errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// 時間戳記
    /// </summary>
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    #endregion
} 