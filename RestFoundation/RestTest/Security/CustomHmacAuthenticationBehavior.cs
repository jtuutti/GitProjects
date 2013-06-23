using System;
using RestFoundation;
using RestFoundation.Behaviors;
using RestFoundation.Security;

namespace RestTest.Security
{
    public class CustomHmacAuthenticationBehavior : HmacAuthenticationBehavior
    {
        public CustomHmacAuthenticationBehavior() : base(HashAlgorithmType.Sha1)
        {
        }

        protected override bool TryGetRequestedSignature(IHttpRequest request, out string userId, out string signature)
        {
            userId = request.QueryString.TryGet("apiuser");
            signature = request.QueryString.TryGet("apisig");

            if ((userId == null || signature == null) && request.Headers.Authorization != null)
            {
                Tuple<string, string> credentials = GetSignatureFromAuthorizationHeader(request);

                userId = credentials.Item1;
                signature = credentials.Item2;
            }

            return !String.IsNullOrWhiteSpace(signature) && !String.IsNullOrWhiteSpace(signature);
        }

        protected override string GenerateServerSignature(IServiceContext context)
        {
            return context.Request.Url.GetLeftPart(UriPartial.Path).TrimEnd(' ', '/', '?', '#').ToLowerInvariant();
        }

        protected override string GetUserPrivateKey(string userId, IHttpRequest request)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            return "SECRET_" + userId.Trim().ToUpperInvariant();
        }

        private static Tuple<string, string> GetSignatureFromAuthorizationHeader(IHttpRequest request)
        {
            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(request.Headers.Authorization, out header) ||
                !String.Equals("HMAC", header.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                return new Tuple<string, string>(null, null);
            }

            return Tuple.Create(header.UserName, header.Parameters.Get("sig"));
        }
    }
}