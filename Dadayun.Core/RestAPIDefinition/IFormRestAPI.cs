using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    /// <summary>
    /// 定义单据RestAPI
    /// </summary>
    public interface IFormRestAPI
    {
        //[Get("/v1/form/templates")]
        //Task<List<Dictionary<string, object>>> GetFormTemplatesAsync();

        [Get("/v1/form/templates/{idOrName}/instances")]
        Task<HttpResponseMessage> GetFormInstancesAsync(string idOrName, string fields = null, string filter = null, int start = 0, int limit = 20, string sort = null, string keyOption = "Entity", bool count = false);

        [Post("/v1/form/templates/{idOrName}/instances?containsAuthority={containsAuthority}")]
        Task<T> AddFormInstanceAsync<T>(string idOrName, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> postData, bool containsAuthority = true);


        [Post("/v1/form/templates/{idOrName}/instances?containsAuthority={containsAuthority}")]
        Task<T> UpdateFormInstanceAsync<T>(string idOrName, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> postData, bool containsAuthority = true);

        [Get("/v1/form/templates/{idOrName}/instances/{instanceId}?keyOption={keyOption}&containsAuthority={containsAuthority}")]
        Task<T> GetFormInstanceAsync<T>(string idOrName, Guid instanceId, string keyOption = "Entity", bool containsAuthority = false);
    }
}
