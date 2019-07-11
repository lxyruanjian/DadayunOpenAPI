using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Dadayun.Core
{
    public static class HttpResponseMessageExtensions
    {
        public static string GetHeaderValue(this HttpResponseMessage response, string headerType)
        {
            if (response.Content != null)
            {
                return response.Content.Headers.GetHeaderValue(headerType);
            }

            return "";
        }

        public static string GetHeaderValue(this HttpContentHeaders contentHeaders, string headerType)
        {
            if (contentHeaders != null && contentHeaders.TryGetValues(headerType, out var dataList))
            {
                if (dataList == null || dataList.Count() == 0)
                    return "";

                var data = dataList.FirstOrDefault();
                
                return data ?? "";
            }

            return "";
        }
    }
}
