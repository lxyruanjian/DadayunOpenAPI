using Newtonsoft.Json;

namespace Dadayun.Core
{
    /// <summary>
    /// 访问令牌
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        [JsonProperty("access_token")]
        public string Token
        {
            get;
            internal set;
        }

        /// <summary>
        /// 失效时间（秒）
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn
        {
            get;
            internal set;
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken
        {
            get;
            internal set;
        }

        /// <summary>
        /// 令牌类型
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// 授权类型
    /// </summary>
    public enum GrantType
    {
        /// <summary>
        /// 认证代码
        /// </summary>
        AuthorizationCode,

        /// <summary>
        /// 帐号密码
        /// </summary>
        Password,

        /// <summary>
        /// 刷新标记
        /// </summary>
        RefreshToken
    }
}
