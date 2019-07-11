using System;
using System.Net;

namespace Dadayun.Core.Exceptions
{
    public class DDYExceptionBase : Exception
    {
        public DDYExceptionBase()
            : base()
        {
        }

        public DDYExceptionBase(string code, string message, string requestId, HttpStatusCode statusCode)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.RequestId = requestId;
        }

        public DDYExceptionBase(string code, string message, string requestId, HttpStatusCode statusCode, Exception innerException)
            : base(message, innerException)
        {
            this.StatusCode = statusCode;
            this.Code = code;
            this.RequestId = requestId;
        }

        /// <summary>
        /// HTTP状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 请求Id
        /// </summary>
        public string RequestId { get; set; }

        public override string ToString()
        {
            return $"HTTP状态码：{(int)StatusCode}，请求Id：{RequestId}，错误码：{Code}，错误描述：{Message}";
        }
    }
}
