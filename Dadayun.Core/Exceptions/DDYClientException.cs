using System;
using System.Net;

namespace Dadayun.Core.Exceptions
{
    public class DDYClientException : DDYExceptionBase
    {
        public DDYClientException()
            : base()
        {
        }

        public DDYClientException(string code, string message, string requestId, HttpStatusCode statusCode)
            : base(code, message, requestId, statusCode)
        {
        }

        public DDYClientException(string code, string message, string requestId, HttpStatusCode statusCode, Exception innerException)
            : base(code, message, requestId, statusCode, innerException)
        {
        }

        public override string ToString()
        {
            return $"客户端异常，{base.ToString()}";
        }
    }
}
