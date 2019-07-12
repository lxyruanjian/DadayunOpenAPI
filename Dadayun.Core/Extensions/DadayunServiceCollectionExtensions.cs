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
        public static IServiceCollection AddDadayunClient(this IServiceCollection services, string endPoint, RefitSettings refitSettings = null)
        {
            if (string.IsNullOrWhiteSpace(endPoint)) throw new ArgumentNullException(nameof(endPoint));

            var settings = refitSettings ?? new RefitSettings();

            services.addDadayunRestAPI<IBaseRestAPI>(endPoint, settings);
            services.addDadayunRestAPI<IFormRestAPI>(endPoint, settings);

            services.AddTransient<IBaseAPI, DefaultBaseAPI>();
            services.AddTransient<IFormAPI, DefaultFormAPI>();
            services.AddSingleton<ITokenService, DefaultTokenService>();

            return services;
        }

        private static void addDadayunRestAPI<T>(this IServiceCollection services, string endPoint, RefitSettings settings = null) where T : class
        {
            services.AddRefitClient<T>(settings)
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(endPoint))
                .AddHttpMessageHandler(provider => GetDDYDelegatingHandler(provider));
        }

        public static DelegatingHandler GetDDYDelegatingHandler(IServiceProvider provider)
        {
            var innerHandler = new AuthenticatedHttpClientHandler(httpRequestMessage =>
                {
                    AddRequiredHeaders(httpRequestMessage);

                    var options = provider.GetService<IOptions<DadayunClientOptions>>().Value;
                    if (options != null && !string.IsNullOrWhiteSpace(options.AccessKeyId) && !string.IsNullOrWhiteSpace(options.AccessSecret))
                    {
                        var signer = new DDYSigner();
                        signer.Sign(httpRequestMessage, options.AccessKeyId, options.AccessSecret);
                    }
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
                var requestEndpoint = request.RequestUri;
                var hostHeader = requestEndpoint.Host;
                if (!requestEndpoint.IsDefaultPort)
                    hostHeader += ":" + requestEndpoint.Port;
                request.Headers.Add(HttpHeader.HostHeader, hostHeader);
            }
        }
    }
}
