using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dadayun.RequestForward
{
    public class RequestForwardMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IHttpClientFactory clientFactory;

        private readonly IOptionsMonitor<RequestForwardOptions> requestForwardOptions;

        protected ILogger logger { get; }

        public RequestForwardMiddleware(RequestDelegate next, IHttpClientFactory clientFactory, IOptionsMonitor<RequestForwardOptions> options, ILoggerFactory loggerFactory)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            this.requestForwardOptions = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public async Task Invoke(HttpContext context)
        {
            var options = requestForwardOptions.CurrentValue;
            if (!string.IsNullOrWhiteSpace(options.UpstreamPath) && context.Request.Path.StartsWithSegments(options.UpstreamPath, StringComparison.OrdinalIgnoreCase))
            {
                var httpRequestMessage = await BuildHttpRequestMessage(context.Request);

                var downstreamRequest = new DownstreamRequest(httpRequestMessage);

                downstreamRequest.Scheme = options.DownstreamScheme;
                downstreamRequest.Host = options.DownstreamHost;
                downstreamRequest.Port = options.DownstreamPort;
                downstreamRequest.AbsolutePath = downstreamRequest.AbsolutePath.Replace(options.UpstreamPath,"");
                downstreamRequest.Query = downstreamRequest.Query.Replace(options.UpstreamPath, "");

                var httpClient = clientFactory.CreateClient();
                try
                {
                    var response = await httpClient.SendAsync(downstreamRequest.ToHttpRequestMessage());
                    await SetResponseOnHttpContext(context, response);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Request Forward Error，ErrorMessge：{ex.Message}，StackTrace：{ex.StackTrace}");
                    SetErrorResponseOnContext(context, 400);
                }
                return;
            }

            await next(context);
        }

        #region HttpRequestMessage

        private readonly string[] _unsupportedHeaders = { "host" };

        public async Task<HttpRequestMessage> BuildHttpRequestMessage(HttpRequest request)
        {
            try
            {
                var requestMessage = new HttpRequestMessage()
                {
                    Content = await MapContent(request),
                    Method = MapMethod(request),
                    RequestUri = MapUri(request)
                };

                MapHeaders(request, requestMessage);

                return requestMessage;
            }
            catch (Exception ex)
            {
                logger.LogError($"BuildHttpRequestMessage Error，ErrorMessge：{ex.Message}，StackTrace：{ex.StackTrace}");
                throw;
            }
        }

        private async Task<HttpContent> MapContent(HttpRequest request)
        {
            if (request.Body == null || (request.Body.CanSeek && request.Body.Length <= 0))
            {
                return null;
            }
            
            var content = new ByteArrayContent(await ToByteArray(request.Body));

            if (!string.IsNullOrEmpty(request.ContentType))
            {
                content.Headers
                    .TryAddWithoutValidation("Content-Type", new[] { request.ContentType });
            }

            AddHeaderIfExistsOnRequest("Content-Language", content, request);
            AddHeaderIfExistsOnRequest("Content-Location", content, request);
            AddHeaderIfExistsOnRequest("Content-Range", content, request);
            AddHeaderIfExistsOnRequest("Content-MD5", content, request);
            AddHeaderIfExistsOnRequest("Content-Disposition", content, request);
            AddHeaderIfExistsOnRequest("Content-Encoding", content, request);

            return content;
        }

        private void AddHeaderIfExistsOnRequest(string key, HttpContent content, HttpRequest request)
        {
            if (request.Headers.ContainsKey(key))
            {
                content.Headers
                    .TryAddWithoutValidation(key, request.Headers[key].ToList());
            }
        }

        private HttpMethod MapMethod(HttpRequest request)
        {
            return new HttpMethod(request.Method);
        }

        private Uri MapUri(HttpRequest request)
        {
            return new Uri(request.GetEncodedUrl());
        }

        private void MapHeaders(HttpRequest request, HttpRequestMessage requestMessage)
        {
            foreach (var header in request.Headers)
            {
                if (IsSupportedHeader(header))
                {
                    requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }
        }

        private bool IsSupportedHeader(KeyValuePair<string, StringValues> header)
        {
            return !_unsupportedHeaders.Contains(header.Key.ToLower());
        }

        private async Task<byte[]> ToByteArray(Stream stream)
        {
            using (stream)
            {
                using (var memStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memStream);
                    return memStream.ToArray();
                }
            }
        }

        #endregion

        #region HttpResponseMessage

        private readonly string[] _unsupportedRequestHeaders =
        {
            "Transfer-Encoding"
        };

        public async Task SetResponseOnHttpContext(HttpContext context, HttpResponseMessage response)
        {
            //_removeOutputHeaders.Remove(response.Headers);

            foreach (var httpResponseHeader in response.Headers.Select(x => new Header(x.Key, x.Value)).ToList())
            {
                if(_unsupportedRequestHeaders.Contains(httpResponseHeader.Key))
                {
                    continue;
                }

                AddHeaderIfDoesntExist(context, httpResponseHeader);
            }

            foreach (var httpResponseHeader in response.Content.Headers)
            {
                AddHeaderIfDoesntExist(context, new Header(httpResponseHeader.Key, httpResponseHeader.Value));
            }

            var content = await response.Content.ReadAsStreamAsync();

            if (response.Content.Headers.ContentLength != null)
            {
                AddHeaderIfDoesntExist(context, new Header("Content-Length", new[] { response.Content.Headers.ContentLength.ToString() }));
            }

            SetStatusCode(context, (int)response.StatusCode);

            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = response.ReasonPhrase;

            using (content)
            {
                if (response.StatusCode != HttpStatusCode.NotModified && context.Response.ContentLength != 0)
                {
                    await content.CopyToAsync(context.Response.Body);
                }
            }
        }

        public void SetErrorResponseOnContext(HttpContext context, int statusCode)
        {
            SetStatusCode(context, statusCode);
        }

        private void SetStatusCode(HttpContext context, int statusCode)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = statusCode;
            }
        }

        private static void AddHeaderIfDoesntExist(HttpContext context, Header httpResponseHeader)
        {
            if (!context.Response.Headers.ContainsKey(httpResponseHeader.Key))
            {
                context.Response.Headers.Add(httpResponseHeader.Key, new StringValues(httpResponseHeader.Values.ToArray()));
            }
        }
        #endregion
    }

    public class Header
    {
        public Header(string key, IEnumerable<string> values)
        {
            Key = key;
            Values = values ?? new List<string>();
        }

        public string Key { get; }
        public IEnumerable<string> Values { get; }
    }
}
