using System;

namespace RestFoundation.Test
{
    public static class RouteAssert
    {
        public static HttpMethodBuilder Url(string relativeUrl)
        {
            if (String.IsNullOrEmpty(relativeUrl))
            {
                throw new ArgumentNullException("relativeUrl");
            }

            if (!relativeUrl.TrimStart().StartsWith("~"))
            {
                throw new ArgumentException("Relative URL must start with the tilde sign: ~", "relativeUrl");
            }

            return new HttpMethodBuilder(relativeUrl);
        }
    }
}
