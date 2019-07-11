using Dadayun.Core.Util;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dadayun.Core.Handlers
{
    public class HttpHeaderMessageHandler : DelegatingHandler
    {
        public HttpHeaderMessageHandler(HttpMessageHandler innerHandler = null)
            : base(innerHandler ?? new HttpClientHandler())
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddRequiredHeaders(request);
            AddOptionalHeaders(request);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private void AddRequiredHeaders(HttpRequestMessage request)
        {
            var headers = request.Headers.ToDictionary(
                                                row => row.Key,
                                                row => row.Value.FirstOrDefault());
            request.Headers.Add(HttpHeader.UserAgentHeader, DadayunSDKUtils.SDKUserAgent);
            //if (requestContext.Request.ContentStream != null)
            //    headers[HttpHeader.ContentLengthHeader] = requestContext.Request.ContentStream.Length.ToString(CultureInfo.InvariantCulture);
            request.Headers.Add(HttpHeader.DateHeader, DadayunSDKUtils.FormattedCurrentTimestampRFC822);

            if (!request.Headers.Contains(HttpHeader.HostHeader))
            {
                Uri requestEndpoint = request.RequestUri;
                var hostHeader = requestEndpoint.Host;
                if (!requestEndpoint.IsDefaultPort)
                    hostHeader += ":" + requestEndpoint.Port;
                request.Headers.Add(HttpHeader.HostHeader, hostHeader);
            }
        }

        private void AddOptionalHeaders(HttpRequestMessage request)
        {
        }
    }
}
