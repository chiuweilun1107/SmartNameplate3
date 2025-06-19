//-----
// <copyright file="SmartNameplateConsts.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

namespace Hamastar.SmartNameplate;

/// <summary>
/// SmartNameplate 專案常數
/// </summary>
public static class SmartNameplateConsts
{
    #region Paging Constants

    /// <summary>
    /// 預設每頁資料筆數
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    /// 最大每頁資料筆數
    /// </summary>
    public const int MaxPageSize = 1000;

    #endregion Paging Constants

    #region String Length Constants

    /// <summary>
    /// 一般名稱最大長度
    /// </summary>
    public const int MaxNameLength = 255;

    /// <summary>
    /// 描述最大長度
    /// </summary>
    public const int MaxDescriptionLength = 1000;

    /// <summary>
    /// URL 最大長度
    /// </summary>
    public const int MaxUrlLength = 2048;

    #endregion String Length Constants

    #region Database Constants

    /// <summary>
    /// 資料表前綴
    /// </summary>
    public const string DbTablePrefix = "App";

    /// <summary>
    /// 資料庫結構描述
    /// </summary>
    public const string DbSchema = null;

    #endregion Database Constants

    #region Field Length Constants

    /// <summary>
    /// 標準備註欄位最大長度
    /// </summary>
    public const int MaxNoteLength = 2000;

    /// <summary>
    /// 電子郵件最大長度
    /// </summary>
    public const int MaxEmailLength = 320;

    /// <summary>
    /// 電話號碼最大長度
    /// </summary>
    public const int MaxPhoneLength = 20;

    /// <summary>
    /// 檔案上傳最大大小 (MB)
    /// </summary>
    public const int MaxFileUploadSizeMb = 50;

    #endregion Field Length Constants

    #region Cache Constants

    /// <summary>
    /// 快取鍵前綴
    /// </summary>
    public const string CacheKeyPrefix = "SmartNameplate";

    /// <summary>
    /// 預設快取過期時間 (分鐘)
    /// </summary>
    public const int DefaultCacheExpirationMinutes = 30;

    #endregion Cache Constants
} 