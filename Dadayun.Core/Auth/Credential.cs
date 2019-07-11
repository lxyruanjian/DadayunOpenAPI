using System;

namespace Dadayun.Core.Auth
{
    public class Credential
    {
        public String AccessKeyId { get; set; }
        public String AccessSecret { get; set; }
        
        public Credential(String keyId, String secret)
        {
            this.AccessKeyId = keyId;
            this.AccessSecret = secret;
        }
    }
}
