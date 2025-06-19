//-----
// <copyright file="SmartNameplateDomainErrorCodes.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Team </author>
//-----

namespace Hamastar.SmartNameplate;

/// <summary>
/// SmartNameplate 領域錯誤代碼定義
/// </summary>
public static class SmartNameplateDomainErrorCodes
{
    #region User Error Codes

    /// <summary>
    /// 使用者不存在
    /// </summary>
    public const string UserNotFound = "SmartNameplate:001001";

    /// <summary>
    /// 使用者已存在
    /// </summary>
    public const string UserAlreadyExists = "SmartNameplate:001002";

    /// <summary>
    /// 使用者無效
    /// </summary>
    public const string UserInvalid = "SmartNameplate:001003";

    #endregion User Error Codes

    #region Device Error Codes

    /// <summary>
    /// 裝置不存在
    /// </summary>
    public const string DeviceNotFound = "SmartNameplate:002001";

    /// <summary>
    /// 裝置已存在
    /// </summary>
    public const string DeviceAlreadyExists = "SmartNameplate:002002";

    /// <summary>
    /// 裝置連線失敗
    /// </summary>
    public const string DeviceConnectionFailed = "SmartNameplate:002003";

    /// <summary>
    /// 裝置離線
    /// </summary>
    public const string DeviceOffline = "SmartNameplate:002004";

    #endregion Device Error Codes

    #region Card Error Codes

    /// <summary>
    /// 卡片不存在
    /// </summary>
    public const string CardNotFound = "SmartNameplate:003001";

    /// <summary>
    /// 卡片已存在
    /// </summary>
    public const string CardAlreadyExists = "SmartNameplate:003002";

    /// <summary>
    /// 卡片格式無效
    /// </summary>
    public const string CardFormatInvalid = "SmartNameplate:003003";

    #endregion Card Error Codes

    #region Template Error Codes

    /// <summary>
    /// 模板不存在
    /// </summary>
    public const string TemplateNotFound = "SmartNameplate:004001";

    /// <summary>
    /// 模板已存在
    /// </summary>
    public const string TemplateAlreadyExists = "SmartNameplate:004002";

    /// <summary>
    /// 模板格式無效
    /// </summary>
    public const string TemplateFormatInvalid = "SmartNameplate:004003";

    #endregion Template Error Codes

    #region Group Error Codes

    /// <summary>
    /// 群組不存在
    /// </summary>
    public const string GroupNotFound = "SmartNameplate:005001";

    /// <summary>
    /// 群組已存在
    /// </summary>
    public const string GroupAlreadyExists = "SmartNameplate:005002";

    /// <summary>
    /// 群組包含子項目
    /// </summary>
    public const string GroupHasChildren = "SmartNameplate:005003";

    #endregion Group Error Codes

    #region Deploy Error Codes

    /// <summary>
    /// 部署失敗
    /// </summary>
    public const string DeployFailed = "SmartNameplate:006001";

    /// <summary>
    /// 部署中
    /// </summary>
    public const string DeployInProgress = "SmartNameplate:006002";

    /// <summary>
    /// 部署已取消
    /// </summary>
    public const string DeployCancelled = "SmartNameplate:006003";

    #endregion Deploy Error Codes

    #region Bluetooth Error Codes

    /// <summary>
    /// 藍牙適配器未找到
    /// </summary>
    public const string BluetoothAdapterNotFound = "SmartNameplate:007001";

    /// <summary>
    /// 藍牙連線失敗
    /// </summary>
    public const string BluetoothConnectionFailed = "SmartNameplate:007002";

    /// <summary>
    /// 藍牙傳輸失敗
    /// </summary>
    public const string BluetoothTransferFailed = "SmartNameplate:007003";

    #endregion Bluetooth Error Codes

    #region Security Error Codes

    /// <summary>
    /// 無效的憑證
    /// </summary>
    public const string InvalidCredentials = "SmartNameplate:008001";

    /// <summary>
    /// 權限不足
    /// </summary>
    public const string InsufficientPermissions = "SmartNameplate:008002";

    /// <summary>
    /// 會話已過期
    /// </summary>
    public const string SessionExpired = "SmartNameplate:008003";

    #endregion Security Error Codes

    #region File Error Codes

    /// <summary>
    /// 檔案不存在
    /// </summary>
    public const string FileNotFound = "SmartNameplate:009001";

    /// <summary>
    /// 檔案格式不支援
    /// </summary>
    public const string FileFormatNotSupported = "SmartNameplate:009002";

    /// <summary>
    /// 檔案太大
    /// </summary>
    public const string FileTooLarge = "SmartNameplate:009003";

    #endregion File Error Codes
} 