using Dadayun.Core;
using Dadayun.Core.Auth;
using Dadayun.Core.Handlers;
using Dadayun.Core.Util;
using Microsoft.Extensions.Options;
using Refit;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DadayunServiceCollectionExtensions
    {
        public static IServiceCollection AddDadayunClient(this IServiceCollection services, string endPoint, RefitSettings settings = null)
        {
            services.AddRefitClient<IBaseRestAPI>(settings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(endPoint))
                .AddHttpMessageHandler(provider => GetDDYSignDelegatingHandler(provider));

            services.AddRefitClient<IFormRestAPI>(settings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(endPoint))
                .AddHttpMessageHandler(provider => GetDDYSignDelegatingHandler(provider));

            services.AddTransient<IBaseAPI, DefaultBaseAPI>();
            services.AddTransient<IFormAPI, DefaultFormAPI>();

            return services;
        }

        public static DelegatingHandler GetDDYSignDelegatingHandler(IServiceProvider provider)
        {
            var innerHandler = new AuthenticatedHttpClientHandler(httpRequestMessage =>
                {
                    AddRequiredHeaders(httpRequestMessage);

                    var signer = new DDYSigner();
                    var options = provider.GetService<IOptions<DadayunClientOptions>>().Value;
                    signer.Sign(httpRequestMessage, options.AccessKeyId, options.AccessSecret);
                    return Task.CompletedTask;
                });

            return innerHandler;
        }
        
        /// <summary>
        /// 签名必填Header
        /// </summary>
        /// <param name="request"></param>
        private static void AddRequiredHeaders(HttpRequestMessage request)
        {
            var headers = request.Headers.ToDictionary(
                                                row => row.Key,
                                                row => row.Value.FirstOrDefault());
            request.Headers.Add(HttpHeader.UserAgentHeader, DadayunSDKUtils.SDKUserAgent);

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
    }
}
