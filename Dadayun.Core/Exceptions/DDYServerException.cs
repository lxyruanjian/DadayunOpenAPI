using System;
using System.Net;

namespace Dadayun.Core.Exceptions
{
    public class DDYServerException : DDYExceptionBase
    {
        public DDYServerException()
            : base()
        {
        }

        public DDYServerException(string code, string message, string requestId, HttpStatusCode statusCode)
            : base(code, message, requestId, statusCode)
        {
        }

        public DDYServerException(string code, string message, string requestId, HttpStatusCode statusCode, Exception innerException)
            : base(code, message, requestId, statusCode, innerException)
        {
        }

        public override string ToString()
        {
            return $"服务端异常，{base.ToString()}";
        }
    }
}
