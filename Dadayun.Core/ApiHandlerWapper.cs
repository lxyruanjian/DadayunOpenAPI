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
        /// <summary>
        /// 所有API请求的公共处理，比如AccessToken过期重试；构造异常等
        /// 之所以不使用Refit框架携带的认证token获取方式是因为不能处理重试和自定义异常
        /// </summary>
        /// <returns></returns>
        public static async Task<T> TryCommonApiAsync<T>(Func<string, Task<T>> apiFun, Func<bool, Task<string>> getAccessTokenFunc = null, string accessToken = null, bool retryIfFaild = true)
        {
            accessToken = string.IsNullOrWhiteSpace(accessToken) && getAccessTokenFunc != null ? await getAccessTokenFunc(false) : null;
            if (!string.IsNullOrWhiteSpace(accessToken))
            {//由于Refit定义在参数里面的认证Header不能定义为[Headers("Authorization: Bearer")]，所以目前只能在这里补上
                accessToken = $"Bearer {accessToken}";
            }

            try
            {
                return await apiFun(accessToken);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == HttpStatusCode.Unauthorized && retryIfFaild && !string.IsNullOrWhiteSpace(accessToken))
                {
                    accessToken = getAccessTokenFunc != null ? $"Bearer {await getAccessTokenFunc(true)}" : null;
                    return await TryCommonApiAsync(apiFun, getAccessTokenFunc, accessToken, false);
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
