using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dadayun.Core.Util
{
    internal abstract class HttpHeader
    {
        public const string ContentTypeHeader = "Content-Type";
        public const string ContentLengthHeader = "Content-Length";
        public const string ContentMD5Header = "Content-MD5";
        public const string AuthorizationHeader = "Authorization";
        public const string SecurityToken = "security-token";


        public const string UserAgentHeader = "User-Agent";
        public const string LocationHeader = "Location";
        public const string HostHeader = "Host";
        public const string DateHeader = "Date";
        public const string AcceptHeader = "Accept";

        public const string XMnsTotalCount = "Total-Count";
        public const string XMnsRequestId = "x-ddy-request-id";
    }
}
