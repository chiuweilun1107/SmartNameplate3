using System.Security.Cryptography;
using System.Text;

namespace SmartNameplate.Api.Services
{
    /// <summary>
    /// 🔐 安全金鑰管理服務實作
    /// 提供企業級的金鑰管理功能
    /// </summary>
    public class KeyManagementService : IKeyManagementService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeyManagementService> _logger;
        private readonly IWebHostEnvironment _environment;

        public KeyManagementService(
            IConfiguration configuration,
            ILogger<KeyManagementService> logger,
            IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// 🛡️ 取得 JWT 密鑰 - 優先從環境變數取得
        /// </summary>
        public string GetJwtSecretKey()
        {
            // 🔐 優先順序：環境變數 > Azure Key Vault > 配置檔案
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                           ?? GetFromAzureKeyVault("jwt-secret-key")
                           ?? _configuration["JwtSettings:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("JWT SecretKey not found in any secure location");
                throw new InvalidOperationException("JWT SecretKey not configured properly");
            }

            // 🛡️ 驗證金鑰強度
            if (!ValidateKeyStrength(secretKey))
            {
                _logger.LogError("JWT SecretKey does not meet security requirements");
                throw new InvalidOperationException("JWT SecretKey is too weak");
            }

            // 🚨 在開發環境中警告硬編碼金鑰
            if (_environment.IsDevelopment() && secretKey.Contains("development_only"))
            {
                _logger.LogWarning("🚨 Using development JWT key - DO NOT USE IN PRODUCTION!");
            }

            return secretKey;
        }

        /// <summary>
        /// 🛡️ 取得加密金鑰
        /// </summary>
        public byte[] GetEncryptionKey()
        {
            var keyString = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
                           ?? GetFromAzureKeyVault("encryption-key")
                           ?? _configuration["Security:EncryptionKey"];

            if (string.IsNullOrEmpty(keyString))
            {
                _logger.LogError("Encryption key not found");
                throw new InvalidOperationException("Encryption key not configured");
            }

            // 🚨 開發環境警告
            if (_environment.IsDevelopment() && keyString.Contains("development_only"))
            {
                _logger.LogWarning("🚨 Using development encryption key - DO NOT USE IN PRODUCTION!");
            }

            // 確保金鑰長度為32字節 (AES-256)
            var keyBytes = Encoding.UTF8.GetBytes(keyString);
            if (keyBytes.Length < 32)
            {
                // 使用 PBKDF2 擴展金鑰
                using var pbkdf2 = new Rfc2898DeriveBytes(keyString, new byte[8], 10000, HashAlgorithmName.SHA256);
                return pbkdf2.GetBytes(32);
            }

            return keyBytes.Take(32).ToArray();
        }

        /// <summary>
        /// 🛡️ 取得加密 IV
        /// </summary>
        public byte[] GetEncryptionIV()
        {
            var ivString = Environment.GetEnvironmentVariable("ENCRYPTION_IV")
                          ?? GetFromAzureKeyVault("encryption-iv")
                          ?? _configuration["Security:EncryptionIV"];

            if (string.IsNullOrEmpty(ivString))
            {
                _logger.LogError("Encryption IV not found");
                throw new InvalidOperationException("Encryption IV not configured");
            }

            var ivBytes = Encoding.UTF8.GetBytes(ivString);
            return ivBytes.Take(16).ToArray(); // AES 需要16字節的IV
        }

        /// <summary>
        /// 🛡️ 生成安全的隨機金鑰
        /// </summary>
        public string GenerateSecretKey(int length = 64)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 🛡️ 驗證金鑰強度
        /// </summary>
        public bool ValidateKeyStrength(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;

            // 🔐 基本長度檢查 (至少32字符)
            if (key.Length < 32) return false;

            // 🔐 複雜度檢查
            bool hasLowerCase = key.Any(char.IsLower);
            bool hasUpperCase = key.Any(char.IsUpper);
            bool hasDigit = key.Any(char.IsDigit);
            bool hasSpecialChar = key.Any(c => !char.IsLetterOrDigit(c));

            // 至少要有3種字符類型
            int complexity = (hasLowerCase ? 1 : 0) + (hasUpperCase ? 1 : 0) + 
                           (hasDigit ? 1 : 0) + (hasSpecialChar ? 1 : 0);

            return complexity >= 3;
        }

        /// <summary>
        /// 🛡️ 加密敏感資料
        /// </summary>
        public string EncryptSensitiveData(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext)) return string.Empty;

            try
            {
                using var aes = Aes.Create();
                aes.Key = GetEncryptionKey();
                aes.IV = GetEncryptionIV();

                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                using var writer = new StreamWriter(cs);
                
                writer.Write(plaintext);
                writer.Flush();
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to encrypt sensitive data");
                throw;
            }
        }

        /// <summary>
        /// 🛡️ 解密敏感資料
        /// </summary>
        public string DecryptSensitiveData(string ciphertext)
        {
            if (string.IsNullOrEmpty(ciphertext)) return string.Empty;

            try
            {
                var cipherBytes = Convert.FromBase64String(ciphertext);

                using var aes = Aes.Create();
                aes.Key = GetEncryptionKey();
                aes.IV = GetEncryptionIV();

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt sensitive data");
                throw;
            }
        }

        /// <summary>
        /// 🛡️ 安全地比較金鑰 (防止時序攻擊)
        /// </summary>
        public bool SecureKeyCompare(string key1, string key2)
        {
            if (key1 == null || key2 == null) return false;
            if (key1.Length != key2.Length) return false;

            var result = 0;
            for (int i = 0; i < key1.Length; i++)
            {
                result |= key1[i] ^ key2[i];
            }

            return result == 0;
        }

        /// <summary>
        /// 🔐 從 Azure Key Vault 取得金鑰 (生產環境推薦)
        /// </summary>
        private string? GetFromAzureKeyVault(string keyName)
        {
            // TODO: 在生產環境中實作 Azure Key Vault 整合
            // 範例：
            // try
            // {
            //     var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
            //     var secret = await client.GetSecretAsync(keyName);
            //     return secret.Value.Value;
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogWarning(ex, "Failed to retrieve key from Azure Key Vault: {KeyName}", keyName);
            //     return null;
            // }

            _logger.LogDebug("Azure Key Vault not configured - using fallback method for key: {KeyName}", keyName);
            return null;
        }
    }
} 