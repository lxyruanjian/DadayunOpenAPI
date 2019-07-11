using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dadayun.Core.Handlers
{
    public class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        readonly Func<HttpRequestMessage, Task> requestSign;

        public AuthenticatedHttpClientHandler(Func<HttpRequestMessage, Task> requestSign)
        {
            this.requestSign = requestSign ?? throw new ArgumentNullException(nameof(requestSign));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await requestSign(request).ConfigureAwait(false);

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
