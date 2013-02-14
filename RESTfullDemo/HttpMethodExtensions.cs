using System;

namespace RESTfullDemo
{
    public static class HttpMethodExtensions
    {
        public static HttpMethod ToHttpMethod(this string httpMethod)
        {
            if (string.IsNullOrEmpty(httpMethod))
            {
                return HttpMethod.Unknown;
            }

            return (HttpMethod)Enum.Parse(typeof(HttpMethod), httpMethod, true);
        }
    }
}