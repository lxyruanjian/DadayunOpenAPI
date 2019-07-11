using Dadayun.Core.Exceptions;
using Dadayun.Core.Util;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    public class ApiHandlerWapper
    {
        public static async Task<T> TryCommonApiAsync<T>(Func<string, Task<T>> apiFun,Func<bool, string> getAccessTokenFunc = null, bool retryIfFaild = true)
        {
            var ak = getAccessTokenFunc != null ? getAccessTokenFunc(false) : "";
            
            try
            {
                return await apiFun(ak);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Unauthorized && retryIfFaild)
                {
                    ak = getAccessTokenFunc != null ? getAccessTokenFunc(true) : "";
                    return await TryCommonApiAsync(apiFun, getAccessTokenFunc, false);
                }
                else if (ex.HasContent)
                {
                    var errorModel = JsonConvert.DeserializeObject<ErrorModel>(ex.Content);
                    var intStatusCode = (int)ex.StatusCode;
                    if (intStatusCode >= 400 && intStatusCode < 500)
                    {
                        throw new DDYClientException(errorModel.Code, errorModel.Message, ex.ContentHeaders.GetHeaderValue(HttpHeader.XMnsRequestId), ex.StatusCode);
                    }
                    else if (intStatusCode >= 500)
                    {
                        throw new DDYServerException(errorModel.Code, errorModel.Message, ex.ContentHeaders.GetHeaderValue(HttpHeader.XMnsRequestId), ex.StatusCode);
                    }
                    throw;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
