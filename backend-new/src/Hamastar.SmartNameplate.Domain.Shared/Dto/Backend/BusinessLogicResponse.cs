//-----
// <copyright file="BusinessLogicResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

using System;
using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend;

/// <summary>
/// 業務邏輯回應基底類別
/// </summary>
/// <typeparam name="T"> 資料類型 </typeparam>
public class BusinessLogicResponse<T>
{
    #region Properties

    /// <summary>
    /// 回應狀態 (success/error)
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; } = "success";

    /// <summary>
    /// 回應訊息
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = "";

    /// <summary>
    /// 回應資料
    /// </summary>
    [JsonProperty("data")]
    public T? Data { get; set; }

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    [JsonProperty("errorCode")]
    public string ErrorCode { get; set; } = "";

    /// <summary>
    /// 時間戳記
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    #endregion Properties

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicResponse{T}" /> class
    /// </summary>
    public BusinessLogicResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicResponse{T}" /> class
    /// </summary>
    /// <param name="data"> 回應資料 </param>
    public BusinessLogicResponse(T data)
    {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicResponse{T}" /> class
    /// </summary>
    /// <param name="status"> 回應狀態 </param>
    /// <param name="message"> 回應訊息 </param>
    /// <param name="data"> 回應資料 </param>
    public BusinessLogicResponse(string status, string message, T? data = default)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    #endregion Constructor

    #region Static Methods

    /// <summary>
    /// 建立成功回應
    /// </summary>
    /// <param name="data"> 回應資料 </param>
    /// <param name="message"> 回應訊息 </param>
    /// <returns> 成功回應 </returns>
    public static BusinessLogicResponse<T> Success(T data, string message = "操作成功")
    {
        return new BusinessLogicResponse<T>("success", message, data);
    }

    /// <summary>
    /// 建立錯誤回應
    /// </summary>
    /// <param name="message"> 錯誤訊息 </param>
    /// <param name="errorCode"> 錯誤代碼 </param>
    /// <returns> 錯誤回應 </returns>
    public static BusinessLogicResponse<T> Error(string message, string errorCode = "")
    {
        return new BusinessLogicResponse<T>
        {
            Status = "error",
            Message = message,
            ErrorCode = errorCode
        };
    }

    #endregion Static Methods
}

/// <summary>
/// 無資料的業務邏輯回應
/// </summary>
public class BusinessLogicResponse : BusinessLogicResponse<object>
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicResponse" /> class
    /// </summary>
    public BusinessLogicResponse() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessLogicResponse" /> class
    /// </summary>
    /// <param name="status"> 回應狀態 </param>
    /// <param name="message"> 回應訊息 </param>
    public BusinessLogicResponse(string status, string message) : base(status, message)
    {
    }

    #endregion Constructor

    #region Static Methods

    /// <summary>
    /// 建立成功回應
    /// </summary>
    /// <param name="message"> 回應訊息 </param>
    /// <returns> 成功回應 </returns>
    public static BusinessLogicResponse Success(string message = "操作成功")
    {
        return new BusinessLogicResponse("success", message);
    }

    /// <summary>
    /// 建立錯誤回應
    /// </summary>
    /// <param name="message"> 錯誤訊息 </param>
    /// <param name="errorCode"> 錯誤代碼 </param>
    /// <returns> 錯誤回應 </returns>
    public static new BusinessLogicResponse Error(string message, string errorCode = "")
    {
        return new BusinessLogicResponse
        {
            Status = "error",
            Message = message,
            ErrorCode = errorCode
        };
    }

    #endregion Static Methods
} 