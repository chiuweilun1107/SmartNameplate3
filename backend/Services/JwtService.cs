using Microsoft.IdentityModel.Tokens;
using SmartNameplate.Api.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartNameplate.Api.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;
        private readonly IKeyManagementService _keyManagementService;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expireMinutes;

        public JwtService(
            IConfiguration configuration, 
            ILogger<JwtService> logger,
            IKeyManagementService keyManagementService)
        {
            _configuration = configuration;
            _logger = logger;
            _keyManagementService = keyManagementService;
            
            // 🛡️ 安全地取得 JWT 設定
            _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
                     ?? _configuration["JwtSettings:Issuer"] 
                     ?? "SmartNameplate";
            
            _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
                       ?? _configuration["JwtSettings:Audience"] 
                       ?? "SmartNameplateUsers";
            
            _expireMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRE_MINUTES"), out var envExpire) 
                            ? envExpire 
                            : _configuration.GetValue<int>("JwtSettings:ExpireMinutes", 60);
        }

        public string GenerateToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _keyManagementService.GetJwtSecretKey();
                var key = Encoding.UTF8.GetBytes(secretKey);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.UserName),
                    new(ClaimTypes.Role, user.Role),
                    new("userId", user.Id.ToString()),
                    new("username", user.UserName),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_expireMinutes),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key), 
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("JWT token generated for user {UserId} ({Username})", 
                    user.Id, user.UserName);

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for user {UserId}", user.Id);
                throw;
            }
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _keyManagementService.GetJwtSecretKey();
                var key = Encoding.UTF8.GetBytes(secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                if (validatedToken is not JwtSecurityToken jwtToken || 
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("Invalid JWT token algorithm");
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("JWT token expired");
                return null;
            }
            catch (SecurityTokenValidationException ex)
            {
                _logger.LogWarning(ex, "JWT token validation failed");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token");
                return null;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            if (principal == null) return null;

            var userIdClaim = principal.FindFirst("userId") ?? principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            return null;
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true; // 如果無法解析，則視為過期
            }
        }
    }
} 