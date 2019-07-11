using Dadayun.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Dadayun.Core.Auth
{
    public class DDYSigner
    {
        #region Immutable Properties

        static readonly Regex CompressWhitespaceRegex = new Regex("\\s+");
        const SigningAlgorithm SignerAlgorithm = SigningAlgorithm.HmacSHA1;

        #endregion

        #region Public Signing Methods

        //public void Sign(IRequest request,
        //                          string accessKeyId,
        //                          string secretAccessKey)
        //{
        //    var signingRequest = SignRequest(request, secretAccessKey);
        //    var signingResult = new StringBuilder();
        //    signingResult.AppendFormat("{0} {1}:{2}",
        //                                     DDYConstants.DDY_AUTHORIZATION_HEADER_PREFIX,
        //                                     accessKeyId,
        //                                     signingRequest);
        //    request.Headers[HttpHeader.AuthorizationHeader] = signingResult.ToString();
        //}

        //public string SignRequest(IRequest request,
        //                                     string secretAccessKey)
        //{
        //    InitializeHeaders(request.Headers);

        //    var parametersToCanonicalize = GetParametersToCanonicalize(request);
        //    var canonicalParameters = CanonicalizeQueryParameters(parametersToCanonicalize);
        //    var canonicalResource = CanonicalizeResource(canonicalParameters, request.ResourcePath);
        //    var canonicalDDYHeaders = CanonoicalizeDDYHeaders(request.Headers);

        //    var canonicalRequest = CanonicalizeRequest(request.HttpMethod,
        //                                               request.Headers,
        //                                               canonicalDDYHeaders,
        //                                               canonicalResource);

        //    return ComputeSignature(secretAccessKey, canonicalRequest);
        //}


        public void Sign(HttpRequestMessage request,
                                  string accessKeyId,
                                  string secretAccessKey)
        {
            var signingRequest = SignRequest(request, secretAccessKey);
            var signingResult = new StringBuilder();
            signingResult.AppendFormat("{0} {1}:{2}",
                                             DDYConstants.DDY_AUTHORIZATION_HEADER_PREFIX,
                                             accessKeyId,
                                             signingRequest);
            request.Headers.Add(HttpHeader.AuthorizationHeader, signingResult.ToString());
        }

        public string SignRequest(HttpRequestMessage request,
                                             string secretAccessKey)
        {
            var headers = request.Headers.ToDictionary(
                                                row => row.Key,
                                                row => row.Value.FirstOrDefault());
            if (request.Content != null)
            {
                var contentHeaders= request.Content.Headers.ToDictionary(
                                                row => row.Key,
                                                row => row.Value.FirstOrDefault());
                foreach (var contentHeader in contentHeaders)
                {
                    if (!headers.ContainsKey(contentHeader.Key))
                    {
                        headers.Add(contentHeader.Key, contentHeader.Value);
                    }
                }
            }
            InitializeHeaders(headers);

            var parametersToCanonicalize = GetParametersToCanonicalize(request);
            var canonicalParameters = CanonicalizeQueryParameters(parametersToCanonicalize);
            var canonicalResource = CanonicalizeResource(canonicalParameters, request.RequestUri.AbsolutePath);
            var canonicalDDYHeaders = CanonoicalizeDDYHeaders(headers);

            var canonicalRequest = CanonicalizeRequest(request.Method.ToString(),
                                                       headers,
                                                       canonicalDDYHeaders,
                                                       canonicalResource);

            return ComputeSignature(secretAccessKey, canonicalRequest);
        }

        #endregion

        #region Public Signing Helpers

        /// <summary>
        /// Initializes request headers.
        /// </summary>
        /// <param name="headers">The request headers</param>
        private static void InitializeHeaders(IDictionary<string, string> headers)
        {
            // clean up any prior signature in the headers if resigning
            headers.Remove(HttpHeader.AuthorizationHeader);
        }

        /// <summary>
        /// Computes and returns an Service signature for the specified canonicalized request
        /// </summary>
        /// <param name="secretAccessKey"></param>
        /// <param name="canonicalRequest"></param>
        /// <returns></returns>
        public static string ComputeSignature(string secretAccessKey, string canonicalRequest)
        {
            return ComputeKeyedHash(SignerAlgorithm, secretAccessKey, canonicalRequest);
        }

        /// <summary>
        /// Compute and return the hash of a data blob using the specified key
        /// </summary>
        /// <param name="algorithm">Algorithm to use for hashing</param>
        /// <param name="key">Hash key</param>
        /// <param name="data">Data blob</param>
        /// <returns>Hash of the data</returns>
        public static string ComputeKeyedHash(SigningAlgorithm algorithm, string key, string data)
        {
            return CryptoUtilFactory.CryptoInstance.HMACSign(data, key, algorithm);
        }

        #endregion

        #region Private Signing Helpers

        protected static string CanonoicalizeDDYHeaders(IDictionary<string, string> headers)
        {
            var headersToCanonicalize = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers.Where(header =>
                    header.Key.ToLowerInvariant().StartsWith(DDYConstants.X_DDY_HEADER_PREFIX)))
                {
                    headersToCanonicalize.Add(header.Key.ToLowerInvariant(), header.Value);
                }
            }
            return CanonicalizeHeaders(headersToCanonicalize);
        }

        protected static string CanonicalizeResource(string canonicalQueryString, string resourcePath)
        {
            var canonicalResource = new StringBuilder();
            canonicalResource.Append(CanonicalizeResourcePath(resourcePath));
            if (canonicalQueryString != string.Empty)
                canonicalResource.AppendFormat("?{0}", canonicalQueryString);
            return canonicalResource.ToString();
        }

        /// <summary>
        /// Returns the canonicalized resource path for the service endpoint
        /// </summary>
        /// <param name="resourcePath">Resource path for the request</param>
        /// <returns>Canonicalized resource path for the endpoint</returns>
        protected static string CanonicalizeResourcePath(string resourcePath)
        {
            if (string.IsNullOrEmpty(resourcePath) || resourcePath == "/")
                return "/";

            var pathSegments = resourcePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var canonicalizedPath = new StringBuilder();
            foreach (var segment in pathSegments)
            {
                canonicalizedPath.AppendFormat("/{0}", segment);
            }

            if (resourcePath.EndsWith("/", StringComparison.Ordinal))
                canonicalizedPath.Append("/");

            return canonicalizedPath.ToString();
        }

        /// <summary>
        /// Computes and returns the canonical request.
        /// </summary>
        /// <param name="httpMethod">The http method used for the request</param>
        /// <param name="headers">The entire request headers</param>
        /// <param name="canonicalDDYHeaders">The canonicalDDYHeaders for the request</param>
        /// <param name="canonicalResource">The canonicalResource for the request</param>
        /// <returns>Canonicalised request as a string</returns>
        protected static string CanonicalizeRequest(string httpMethod,
                                                    IDictionary<string, string> headers,
                                                    string canonicalDDYHeaders,
                                                    string canonicalResource)
        {
            var canonicalRequest = new StringBuilder();
            canonicalRequest.AppendFormat("{0}\n", httpMethod);

            var contentMD5 = string.Empty;
            if (headers.ContainsKey(HttpHeader.ContentMD5Header))
                contentMD5 = headers[HttpHeader.ContentMD5Header];
            canonicalRequest.AppendFormat("{0}\n", contentMD5);

            var contentType = string.Empty;
            if (headers.ContainsKey(HttpHeader.ContentTypeHeader))
                contentType = headers[HttpHeader.ContentTypeHeader];
            canonicalRequest.AppendFormat("{0}\n", contentType);

            canonicalRequest.AppendFormat("{0}\n", headers[HttpHeader.DateHeader]);
            canonicalRequest.Append(canonicalDDYHeaders);
            canonicalRequest.Append(canonicalResource);

            return canonicalRequest.ToString();
        }

        /// <summary>
        /// Computes the canonical headers with values for the request.
        /// </summary>
        /// <param name="sortedHeaders">All sorted request headers</param>
        /// <returns>The canonical headers.</returns>
        protected static string CanonicalizeHeaders(ICollection<KeyValuePair<string, string>> sortedHeaders)
        {
            if (sortedHeaders == null || sortedHeaders.Count == 0)
                return string.Empty;

            var builder = new StringBuilder();
            foreach (var entry in sortedHeaders)
            {
                builder.Append(entry.Key.ToLower(CultureInfo.InvariantCulture));
                builder.Append(":");
                builder.Append(CompressSpaces(entry.Value));
                builder.Append("\n");
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// Collects all subresources and query string parameters. 
        /// </summary>
        /// <param name="request">The request being signed</param>
        /// <returns>A set of parameters</returns>
        protected static IDictionary<string, string> GetParametersToCanonicalize(HttpRequestMessage request)
        {
            var parametersToCanonicalize = new Dictionary<string, string>();
            var queryString = request.RequestUri.Query;

            var parameters = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(queryString))
            {
                if (queryString.StartsWith("?"))
                    queryString = queryString.TrimStart('?');

                var parameterList = queryString.Split('&');
                foreach (var item in parameterList)
                {
                    var paramList = item.Split('=');
                    if (paramList.Count() > 0)
                        parameters.Add(paramList[0], paramList[1]);
                }
            }

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var queryParameter in parameters.Where(queryParameter => queryParameter.Value != null))
                {
                    parametersToCanonicalize.Add(queryParameter.Key, queryParameter.Value);
                }
            }

            return parametersToCanonicalize;
        }

        /// <summary>
        /// Computes and returns the canonicalized query string
        /// </summary>
        /// <param name="parameters">The set of query string parameters</param>
        /// <returns>The canonical query string parameters</returns>
        protected static string CanonicalizeQueryParameters(IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return string.Empty;

            var canonicalQueryString = new StringBuilder();
            var queryParams = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            foreach (var p in queryParams)
            {
                if (canonicalQueryString.Length > 0)
                    canonicalQueryString.Append("&");
                if (string.IsNullOrEmpty(p.Value))
                    canonicalQueryString.AppendFormat("{0}=", p.Key);
                else
                    canonicalQueryString.AppendFormat("{0}={1}", p.Key, p.Value);
            }

            return canonicalQueryString.ToString();
        }

        static string CompressSpaces(string data)
        {
            if (data == null || !data.Contains(" "))
                return data;

            var compressed = CompressWhitespaceRegex.Replace(data, " ");
            return compressed;
        }

        #endregion
    }

    /// <summary>
    /// The valid hashing algorithm supported by DDY for request signing.
    /// </summary>
    public enum SigningAlgorithm
    {
        HmacSHA1,
        HmacSHA256
    };

    /// <summary>
    /// The http methods supported by DDY.
    /// </summary>
    public enum HttpMethod
    {
        GET,
        PUT,
        POST,
        DELETE
    }
}
