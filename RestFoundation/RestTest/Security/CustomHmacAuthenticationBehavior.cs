using System;
using RestFoundation;
using RestFoundation.Behaviors;
using RestFoundation.Security;

namespace RestTest.Security
{
    public class CustomHmacAuthenticationBehavior : HmacAuthenticationBehavior
    {
        public CustomHmacAuthenticationBehavior() : base(HashAlgorithmType.Md5)
        {
        }

        protected override bool TryGetRequestedSignature(IHttpRequest request, out string signature)
        {
            signature = request.QueryString.TryGet("apikey");

            if (signature == null && request.Headers.Authorization != null)
            {
                signature = GetSignatureFromAuthorizationHeader(request);
            }

            return !String.IsNullOrEmpty(signature);
        }

        protected override string GenerateServerSignature(IServiceContext context)
        {
            return context.Request.Url.GetLeftPart(UriPartial.Path).ToLowerInvariant();
        }

        protected override string GetUserPrivateKey(IHttpRequest request)
        {
            return "secret";
        }

        private static string GetSignatureFromAuthorizationHeader(IHttpRequest request)
        {
            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(request.Headers.Authorization, out header) ||
                !String.Equals("HMAC", header.AuthenticationType, StringComparison.OrdinalIgnoreCase) ||
                String.IsNullOrWhiteSpace(header.Password))
            {
                return null;
            }

            return header.Password;
        }
    }
}