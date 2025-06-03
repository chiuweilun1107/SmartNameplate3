using SmartNameplate.Api.Entities;
using System.Security.Claims;

namespace SmartNameplate.Api.Services
{
    public interface IJwtService
    {
        /// <summary>
        /// 生成 JWT Token
        /// </summary>
        string GenerateToken(User user);
        
        /// <summary>
        /// 驗證 Token 並取得用戶 Claims
        /// </summary>
        ClaimsPrincipal? ValidateToken(string token);
        
        /// <summary>
        /// 從 Token 取得用戶 ID
        /// </summary>
        int? GetUserIdFromToken(string token);
        
        /// <summary>
        /// 檢查 Token 是否過期
        /// </summary>
        bool IsTokenExpired(string token);
    }
} 