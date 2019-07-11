using Refit;
using System;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    /// <summary>
    /// 定义基础RestAPI
    /// </summary>
    public interface IBaseRestAPI
    {
        [Get("/v1/date")]
        Task<DateTime> GetDateAsync();
    }
}
