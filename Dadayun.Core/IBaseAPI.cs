using System;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    public interface IBaseAPI
    {
        Task<DateTime> GetDateAsync();
    }
}
