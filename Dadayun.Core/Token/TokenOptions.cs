using System.Collections.Generic;

namespace Dadayun.Core
{
    public class TokenOptions
    {
        public string Address { get; set; }
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }

        public string Code { get; set; }
        public string RedirectUri { get; set; }
        public string CodeVerifier { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
