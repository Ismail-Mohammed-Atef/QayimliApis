using Microsoft.AspNetCore.Http;

namespace Qayimli.APIs.Helpers
{
    public static class RequestHeaderHelper
    {
        public static string GetLanguageFromHeader(HttpRequest request)
        {
            if (request != null)
            {
                if (request.Headers.TryGetValue("lang", out var langHeader))
                {
                    return langHeader.ToString().ToLower();
                }
            }
            return "en";
        }
    }
}
