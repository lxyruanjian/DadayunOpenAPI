using System;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    public interface ITokenService
    {
        Func<bool, Task<string>> AccessTokenGetter { get; }

        Task<string> GetAccessTokenAsync(bool getNew = false);
    }
}
