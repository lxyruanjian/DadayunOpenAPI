using System;

namespace Dadayun.Core
{
    public class Token
    {
        public Token(string accessToken, string idToken, int expiresIn, string tokenType, string refreshToken)
        {
            AccessToken = accessToken;
            IdToken = idToken;
            ExpiresIn = expiresIn;
            TokenType = tokenType;
            RefreshToken = refreshToken;

            AccessTokenExpireAt = DateTime.UtcNow.AddSeconds(ExpiresIn);
        }

        public string AccessToken { get; private set; }
        public string IdToken { get; private set; }
        public int ExpiresIn { get; private set; }
        public string TokenType { get; private set; }
        public string RefreshToken { get; private set; }

        /// <summary>
        /// AccessToken过期UTC时间
        /// </summary>
        public DateTime AccessTokenExpireAt { get; private set; }

        /// <summary>
        /// 是否已过期
        /// </summary>
        public bool IsExpire
        {
            get
            {
                return AccessTokenExpireAt < DateTime.UtcNow;
            }
        }
    }
}
