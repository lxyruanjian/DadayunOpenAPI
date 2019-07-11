using Dadayun.Core.Auth;

namespace Dadayun.Core.Profile
{
    public interface IClientProfile
    {
        Credential GetCredential();
    }

    public class DefaultClientProfile : IClientProfile
    {
        private Credential credential;

        public DefaultClientProfile(string accessKeyId, string secretAccessKey) : this(new Credential(accessKeyId, secretAccessKey))
        {

        }

        public DefaultClientProfile(Credential credential)
        {
            this.credential = credential;
        }

        public Credential GetCredential()
        {
            return credential;
        }
    }
}
