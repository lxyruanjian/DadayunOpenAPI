using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dadayun.Core
{
    public interface OAuthRestAPI
    {
        [Post("/connect/token")]
        Task<AccessToken> GetAccessTokenByPasswordAsync(string idOrName, string fields = null, string filter = null, int start = 0, int limit = 20, string sort = null, string keyOption = "Entity", bool count = false);
    }
}
