using System.Security.Cryptography;

namespace SmartNameplate.Api.Services
{
    /// <summary>
    /// 🔐 金鑰管理服務介面
    /// 提供安全的金鑰生成、儲存和檢索功能
    /// </summary>
    public interface IKeyManagementService
    {
        /// <summary>
        /// 🛡️ 取得 JWT 密鑰
        /// </summary>
        string GetJwtSecretKey();
        
        /// <summary>
        /// 🛡️ 取得加密金鑰
        /// </summary>
        byte[] GetEncryptionKey();
        
        /// <summary>
        /// 🛡️ 取得加密 IV
        /// </summary>
        byte[] GetEncryptionIV();
        
        /// <summary>
        /// 🛡️ 生成安全的隨機金鑰
        /// </summary>
        string GenerateSecretKey(int length = 64);
        
        /// <summary>
        /// 🛡️ 驗證金鑰強度
        /// </summary>
        bool ValidateKeyStrength(string key);
        
        /// <summary>
        /// 🛡️ 加密敏感資料
        /// </summary>
        string EncryptSensitiveData(string plaintext);
        
        /// <summary>
        /// 🛡️ 解密敏感資料
        /// </summary>
        string DecryptSensitiveData(string ciphertext);
        
        /// <summary>
        /// 🛡️ 安全地比較金鑰
        /// </summary>
        bool SecureKeyCompare(string key1, string key2);
    }
} 