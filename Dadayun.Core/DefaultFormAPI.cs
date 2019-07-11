using Dadayun.Core.Auth;
using Dadayun.Core.Exceptions;
using Dadayun.Core.Handlers;
using Dadayun.Core.RequestDto;
using Dadayun.Core.ResponseDto;
using Dadayun.Core.Util;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    /// <summary>
    /// 默认单据接口实现
    /// </summary>
    public class DefaultFormAPI : IFormAPI
    {
        private IFormRestAPI formRestAPI;
        public DefaultFormAPI(IFormRestAPI formRestAPI)
        {
            this.formRestAPI = formRestAPI;
        }

        //private void buildFormAPI()
        //{
        //    HttpMessageHandler innerHandler = new AuthenticatedHttpClientHandler(requestSign, null);
        //    innerHandler = new HttpHeaderMessageHandler(innerHandler);

        //    var client = new HttpClient(innerHandler ?? new HttpClientHandler()) { BaseAddress = new Uri(endPoint) };
        //    formRestAPI = RestService.For<IFormRestAPI>(client);
        //}

        //private DDYSigner signer = new DDYSigner();
        //private Task requestSign(HttpRequestMessage request)
        //{
        //    signer.Sign(request, credential.AccessKeyId, credential.AccessSecret);
        //    return Task.CompletedTask;
        //}

        public Task<SetResult<T>> GetFormInstancesAsync<T>(string idOrName, IEnumerable<string> fields = null, IEnumerable<QueryCondition> filter = null, int start = 0, int limit = 20, ISort sort = null, FormKeyOption keyOption = FormKeyOption.Entity, bool count = false)
        {
            return ApiHandlerWapper.TryCommonApiAsync(async token =>
                {
                    if (start < 0 || start > int.MaxValue)
                    {
                        throw new ArgumentException(nameof(start));
                    }
                    if (limit < 0 || limit > int.MaxValue)
                    {
                        throw new ArgumentException(nameof(limit));
                    }
                    var sortField = "";
                    if (sort != null)
                    {
                        sortField = sort.PropertyName;
                        if (!sort.Ascending)
                        {
                            sortField = "-" + sortField;
                        }
                    }
                    var fieldsStr = fields != null && fields.Count() > 0 ? string.Join(",", fields) : null;
                    var filterStr = filter != null && filter.Count() > 0 ? JsonConvert.SerializeObject(filter.ToQueryConditionRequests()) : null;

                    var httpResponseMessage = await formRestAPI.GetFormInstancesAsync(idOrName, fieldsStr, filterStr, start, limit, sortField, keyOption.ToString(), count).ConfigureAwait(false);

                    return await generateSetResult<T>(start, limit, count, httpResponseMessage);
                },null);
        }

        public Task<T> GetFormInstanceAsync<T>(string idOrName, Guid instanceId, FormKeyOption keyOption = FormKeyOption.Entity, bool containsAuthority = false)
        {
            return ApiHandlerWapper.TryCommonApiAsync(token => formRestAPI.GetFormInstanceAsync<T>(idOrName, instanceId, keyOption.ToString(), containsAuthority));
        }

        public Task<T> AddFormInstanceAsync<T>(string idOrName, object newInstance, FormKeyOption keyOption = FormKeyOption.Entity, bool containsAuthority = true)
        {
            var postData = new Dictionary<string, string>();
            postData.Add("jsonFormData", JsonConvert.SerializeObject(newInstance));
            postData.Add("keyOption", keyOption.ToString());
            return ApiHandlerWapper.TryCommonApiAsync(token => formRestAPI.AddFormInstanceAsync<T>(idOrName, postData, containsAuthority));
        }

        private static async Task<SetResult<T>> generateSetResult<T>(int start, int limit, bool count, HttpResponseMessage httpResponseMessage)
        {
            var setResult = new SetResult<T>();
            setResult.Start = start;
            setResult.Limit = limit;
            try
            {
                var content = httpResponseMessage.Content ?? new StringContent(string.Empty);

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    var apiException = await ApiException.Create(httpResponseMessage.RequestMessage, httpResponseMessage.RequestMessage.Method, httpResponseMessage).ConfigureAwait(false);
                    //if (apiException.HasContent)
                    //{
                    //    //var serverExceptionModel = JsonConvert.DeserializeObject<ErrorModel>(apiException.Content);
                    //    //var requestId = httpResponseMessage.GetHeaderValue(HttpHeader.XMnsRequestId);
                    //    //var serverException = new DDYServerException(serverExceptionModel.Code, serverExceptionModel.Message, requestId, apiException.StatusCode);
                    //    //throw serverException;

                    //    var errorModel = JsonConvert.DeserializeObject<ErrorModel>(apiException.Content);
                    //    var requestId = httpResponseMessage.GetHeaderValue(HttpHeader.XMnsRequestId);
                    //    var intStatusCode = (int)apiException.StatusCode;
                    //    if (intStatusCode >= 400 && intStatusCode < 500)
                    //    {
                    //        throw new DDYClientException(errorModel.Code, errorModel.Message, requestId, apiException.StatusCode);
                    //    }
                    //    else if (intStatusCode >= 500)
                    //    {
                    //        throw new DDYServerException(errorModel.Code, errorModel.Message, requestId, apiException.StatusCode);
                    //    }
                    //}

                    throw apiException;
                }

                using (var stream = await content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var serializer = JsonSerializer.Create();
                        var datas = serializer.Deserialize<IEnumerable<T>>(jsonReader);
                        setResult.Datas = datas;
                        setResult.Count = datas.Count();
                        if (count)
                        {
                            var totalCountStr = httpResponseMessage.GetHeaderValue(HttpHeader.XMnsTotalCount);
                            if (int.TryParse(totalCountStr, out var totalCount))
                            {
                                setResult.TotalCount = totalCount;
                            }
                        }
                    }
                }
            }
            finally
            {
            }
            return setResult;
        }
    }
}
