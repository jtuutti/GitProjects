using System;
using System.Globalization;
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

        protected override bool TryGetRequestedSignature(IHttpRequest request, out string userId, out string signature, out DateTime timestamp)
        {
            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(request.Headers.Authorization, out header) ||
                !String.Equals("HMAC", header.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                userId = null;
                signature = null;
                timestamp = default(DateTime);
                return false;
            }

            userId = header.UserName;
            signature = header.Parameters.Get("sig");

            if (String.IsNullOrWhiteSpace(userId) ||
                String.IsNullOrWhiteSpace(signature) ||
                !DateTime.TryParse(request.Headers.TryGet("Date"), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out timestamp))
            {
                userId = null;
                signature = null;
                timestamp = default(DateTime);
                return false;
            }

            return true;
        }

        protected override bool IsRequestedSignatureValid(IServiceContext serviceContext, string signatureHash, DateTime timestamp)
        {
            return (DateTime.UtcNow - timestamp) <= TimeSpan.FromMinutes(15);
        }

        protected override string GenerateServerSignature(IServiceContext context, string userId, DateTime timespan)
        {
            string urlPaRT = context.Request.Url.GetLeftPart(UriPartial.Path).TrimEnd(' ', '/', '?', '#').ToLowerInvariant();
            string dateTimePart = timespan.ToString("yyyyMMddhhmmss");
            const string salt = "HM@CS1G";

            return urlPaRT + dateTimePart + userId + salt;
        }

        protected override string GetUserPrivateKey(string userId, IHttpRequest request)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            return "SECRET_" + userId.Trim().ToUpperInvariant();
        }
    }
}