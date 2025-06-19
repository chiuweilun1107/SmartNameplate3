#!/bin/bash

# 建立所有缺失的Response檔案
mkdir -p Security/Response
mkdir -p TextTags/Response
mkdir -p Users/Response

# Security Response檔案
cat > Security/Response/LogoutResponse.cs << 'EOF'
//-----
// <copyright file="LogoutResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 登出回應
    /// </summary>
    public class LogoutResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        #endregion Properties
    }
}
EOF

cat > Security/Response/RefreshTokenResponse.cs << 'EOF'
//-----
// <copyright file="RefreshTokenResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 刷新Token回應
    /// </summary>
    public class RefreshTokenResponse
    {
        #region Properties

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("result")]
        public bool Result { get; set; }

        /// <summary>
        /// 新Token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; } = "";

        #endregion Properties
    }
}
EOF

cat > Security/Response/ValidateTokenResponse.cs << 'EOF'
//-----
// <copyright file="ValidateTokenResponse.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Response
{
    /// <summary>
    /// 驗證Token回應
    /// </summary>
    public class ValidateTokenResponse
    {
        #region Properties

        /// <summary>
        /// 是否有效
        /// </summary>
        [JsonProperty("isValid")]
        public bool IsValid { get; set; }

        #endregion Properties
    }
}
EOF

# Security Request檔案
mkdir -p Security/Request

cat > Security/Request/RefreshTokenRequest.cs << 'EOF'
//-----
// <copyright file="RefreshTokenRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 刷新Token請求
    /// </summary>
    public class RefreshTokenRequest
    {
        #region Properties

        /// <summary>
        /// 刷新Token
        /// </summary>
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = "";

        #endregion Properties
    }
}
EOF

cat > Security/Request/ValidateTokenRequest.cs << 'EOF'
//-----
// <copyright file="ValidateTokenRequest.cs" company="Hamastar">
//     Copyright (c) Hamastar. All rights reserved.
// </copyright>
// <author> SmartNameplate Development Team </author>
//-----

using Newtonsoft.Json;

namespace Hamastar.SmartNameplate.Dto.Backend.Security.Request
{
    /// <summary>
    /// 驗證Token請求
    /// </summary>
    public class ValidateTokenRequest
    {
        #region Properties

        /// <summary>
        /// Token
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; } = "";

        #endregion Properties
    }
}
EOF

echo "所有DTO檔案建立完成！" 