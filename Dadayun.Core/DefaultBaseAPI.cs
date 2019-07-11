using System;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    public class DefaultBaseAPI : IBaseAPI
    {
        private IBaseRestAPI baseRestAPI;
        public DefaultBaseAPI(IBaseRestAPI baseRestAPI)
        {
            this.baseRestAPI = baseRestAPI;
        }

        public Task<DateTime> GetDateAsync()
        {
            return ApiHandlerWapper.TryCommonApiAsync(token=>baseRestAPI.GetDateAsync());
        }
    }
}
