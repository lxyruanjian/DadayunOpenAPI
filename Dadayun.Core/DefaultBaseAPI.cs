using System;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    public class DefaultBaseAPI : IBaseAPI
    {
        private readonly IBaseRestAPI baseRestAPI;
        private readonly ITokenService tokenService;
        public DefaultBaseAPI(IBaseRestAPI baseRestAPI, ITokenService tokenService)
        {
            this.baseRestAPI = baseRestAPI;
            this.tokenService = tokenService;
        }

        public Task<DateTime> GetDateAsync()
        {
            return ApiHandlerWapper.TryCommonApiAsync(token => baseRestAPI.GetDateAsync(token), tokenService.AccessTokenGetter);
        }
    }
}
