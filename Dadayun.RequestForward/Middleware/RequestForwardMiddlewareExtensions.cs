using Dadayun.RequestForward;

namespace Microsoft.AspNetCore.Builder
{
    public static class RequestForwardMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestForwardMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestForwardMiddleware>();
        }
    }
}
