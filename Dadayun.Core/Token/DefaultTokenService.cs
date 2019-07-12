using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    public class DefaultTokenService : ITokenService
    {
        private IHttpClientFactory httpClientFactory { get; }
        private readonly IOptionsMonitor<TokenOptions> options;
        //private object tokenLockObj = new object();

        //DefaultTokenService注入的是单例，token在这里是内存存储，也可以定义一个store持久化token，以减少token请求次数或者集群环境共享问题；
        private Token token;

        public DefaultTokenService(IHttpClientFactory httpClientFactory, IOptionsMonitor<TokenOptions> options)
        {
            this.httpClientFactory = httpClientFactory;
            this.options = options;
        }

        public Func<bool, Task<string>> AccessTokenGetter
        {
            get
            {
                var tokenOption = options.CurrentValue;
                if (tokenOption != null && !string.IsNullOrWhiteSpace(tokenOption.Address) && !string.IsNullOrWhiteSpace(tokenOption.GrantType) && !string.IsNullOrWhiteSpace(tokenOption.ClientId) && !string.IsNullOrWhiteSpace(tokenOption.ClientSecret))
                {
                    return GetAccessTokenAsync;
                }
                return null;
            }
        }

        public async Task<string> GetAccessTokenAsync(bool getNew = false)
        {
            if (token == null || token.IsExpire || getNew)
            {
                if (!getNew && token != null && !string.IsNullOrWhiteSpace(token.RefreshToken) && token.IsExpire)
                {//token过期
                    token = await RefreshTokenAsync(token.RefreshToken);
                    if (token != null)
                    {
                        return token.AccessToken;
                    }
                }

                switch (options.CurrentValue.GrantType.ToLower())
                {
                    case "code":
                        token = await GetAccessTokenByCodeAsync();
                        break;
                    case "password":
                        token = await GetAccessTokenByPasswordAsync();
                        break;
                    default:
                        break;
                }
            }

            return token?.AccessToken;
        }

        public async Task<Token> GetAccessTokenByCodeAsync()
        {
            var httpClient = httpClientFactory.CreateClient();
            var tokenOption = options.CurrentValue;
            var response = await httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = tokenOption.Address,
                GrantType = tokenOption.GrantType,

                ClientId = tokenOption.ClientId,
                ClientSecret = tokenOption.ClientSecret,

                Code = tokenOption.Code,
                RedirectUri = tokenOption.RedirectUri,

                CodeVerifier = tokenOption.CodeVerifier,

                Parameters = tokenOption.Parameters
            });

            if (!response.IsError)
            {
                return new Token(response.AccessToken, response.IdentityToken, response.ExpiresIn, response.TokenType, response.RefreshToken);
            }
            else
            {
                throw response.Exception;
            }
        }

        public async Task<Token> GetAccessTokenByPasswordAsync()
        {
            var httpClient = httpClientFactory.CreateClient();
            var tokenOption = options.CurrentValue;
            var response = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = tokenOption.Address,
                GrantType = tokenOption.GrantType,

                ClientId = tokenOption.ClientId,
                ClientSecret = tokenOption.ClientSecret,
                Scope = tokenOption.Scope,

                UserName = tokenOption.UserName,
                Password = tokenOption.Password,

                Parameters = tokenOption.Parameters
            });

            if (!response.IsError)
            {
                return new Token(response.AccessToken, response.IdentityToken, response.ExpiresIn, response.TokenType, response.RefreshToken);
            }
            else
            {
                //return null;
                throw response.Exception;
            }
        }

        public async Task<Token> RefreshTokenAsync(string refreshToken)
        {
            var httpClient = httpClientFactory.CreateClient();
            var tokenOption = options.CurrentValue;
            var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = tokenOption.Address,

                ClientId = tokenOption.ClientId,
                ClientSecret = tokenOption.ClientSecret,

                RefreshToken = refreshToken
            });

            if (!response.IsError)
            {
                return new Token(response.AccessToken, response.IdentityToken, response.ExpiresIn, response.TokenType, response.RefreshToken);
            }
            else
            {
                //return null;
                throw response.Exception;
            }
        }
    }
}
