using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    public sealed class DigestAuthorizationBehavior : ServiceSecurityBehavior, IDisposable
    {
        public enum QualityOfProtection
        {
            None,
            Auth
        }

        private const string AuthenticationType = "Digest";
        private const string UserAddressVariableName = "REMOTE_ADDR";
        private const int DefaultNonceLifeTimeInSeconds = 300;

        private readonly IAuthorizationManager m_authorizationManager;
        private readonly MD5Encoder m_encoder;

        public TimeSpan NonceLifetime { get; set; }
        public QualityOfProtection Qop { get; set; }

        public DigestAuthorizationBehavior() : this(Rest.Active.CreateObject<IAuthorizationManager>())
        {
        }

        public DigestAuthorizationBehavior(IAuthorizationManager authorizationManager)
        {
            if (authorizationManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No authorization manager could be found");
            }

            NonceLifetime = TimeSpan.FromSeconds(DefaultNonceLifeTimeInSeconds);
            Qop = QualityOfProtection.Auth;

            m_authorizationManager = authorizationManager;
            m_encoder = new MD5Encoder();
        }

        public override bool OnMethodAuthorizing(IServiceContext context, object service, MethodInfo method)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(context.Request.Headers.Authorization, context.Request.Headers.ContentCharsetEncoding, out header) ||
                !AuthenticationType.Equals(header.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                GenerateAuthenticationHeader(context, false);
                return false;
            }

            Tuple<bool, bool> responseValidationResult = ValidateResponse(context, header);

            if (!responseValidationResult.Item1)
            {
                GenerateAuthenticationHeader(context, responseValidationResult.Item2);
                return false;
            }

            context.User = new GenericPrincipal(new GenericIdentity(header.UserName, AuthenticationType), m_authorizationManager.GetRoles(header.UserName).ToArray());
            return true;
        }
        
        public void Dispose()
        {
            m_encoder.Dispose();
        }

        private void GenerateAuthenticationHeader(IServiceContext context, bool isStale)
        {
            string ipAddress = context.Request.ServerVariables.TryGet(UserAddressVariableName);

            if (String.IsNullOrWhiteSpace(ipAddress))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "Forbidden");
            }

            string timestamp = (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

            var headerBuilder = new StringBuilder();
            headerBuilder.Append(AuthenticationType).Append(' ');
            headerBuilder.AppendFormat(CultureInfo.InvariantCulture, "realm=\"{0}\"", context.Request.Url.ServiceUrl);
            headerBuilder.AppendFormat(CultureInfo.InvariantCulture, ", nonce=\"{0}\"", GenerateNonce(timestamp, ipAddress));

            if (Qop == QualityOfProtection.Auth)
            {
                headerBuilder.AppendFormat(CultureInfo.InvariantCulture, ", qop=\"auth\"");
            }

            if (isStale)
            {
                headerBuilder.AppendFormat(CultureInfo.InvariantCulture, ", stale=\"TRUE\"");
            }

            context.Response.Output.Clear();
            context.Response.SetHeader("WWW-Authenticate", headerBuilder.ToString());
            context.Response.SetStatus(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        private string GenerateNonce(string timestamp, string ipAddress)
        {
            string nonce = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}", timestamp, ipAddress));

            return Base64Converter.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}", timestamp, nonce));
        }

        private Tuple<bool, bool> ValidateResponse(IServiceContext context, AuthorizationHeader header)
        {
            if (String.IsNullOrWhiteSpace(header.UserName) || header.Parameters == null)
            {
                return Tuple.Create(false, false);
            }

            string ipAddress = context.Request.ServerVariables.TryGet(UserAddressVariableName);

            if (String.IsNullOrWhiteSpace(ipAddress))
            {
                return Tuple.Create(false, false);
            }

            string response = header.Parameters.Get("response");

            if (String.IsNullOrWhiteSpace(response))
            {
                return Tuple.Create(false, false);
            }

            string realm = header.Parameters.Get("realm");

            if (String.IsNullOrWhiteSpace(realm))
            {
                return Tuple.Create(false, false);
            }

            string uri = header.Parameters.Get("uri");

            if (String.IsNullOrWhiteSpace(uri))
            {
                return Tuple.Create(false, false);
            }

            string password = m_authorizationManager.GetPassword(header.UserName);

            if (String.IsNullOrEmpty(password))
            {
                return Tuple.Create(false, false);
            }

            string nonce = header.Parameters.Get("nonce");           
            Tuple<bool, bool> nonceValidationResult = ValidateNonce(nonce, ipAddress);

            if (!nonceValidationResult.Item1)
            {
                return nonceValidationResult;
            }

            string ha1 = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", header.UserName, realm, password));
            string ha2 = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}", context.Request.Method.ToString().ToUpperInvariant(), uri));

            string expectedResponse;

            if (Qop == QualityOfProtection.Auth)
            {
                string nonceCounter = header.Parameters.Get("nc");

                if (String.IsNullOrEmpty(nonceCounter))
                {
                    return Tuple.Create(false, false);
                }

                string clientNonce = header.Parameters.Get("cnonce");

                if (String.IsNullOrEmpty(clientNonce))
                {
                    return Tuple.Create(false, false);
                }

                expectedResponse = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}:{3}:{4}:{5}", ha1, nonce, nonceCounter, clientNonce, "auth", ha2));
            }
            else
            {
                expectedResponse = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", ha1, nonce, ha2));
            }

            return Tuple.Create(String.Equals(expectedResponse, response, StringComparison.Ordinal), false);
        }

        private Tuple<bool, bool> ValidateNonce(string nonce, string ipAddress)
        {
            if (String.IsNullOrEmpty(nonce))
            {
                return Tuple.Create(false, false);
            }

            string nonceString;

            try
            {
                nonceString = Base64Converter.Decode(nonce);
            }
            catch (Exception)
            {
                return Tuple.Create(false, false);
            }

            string[] nonceParts = nonceString.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (nonceParts.Length != 2 || nonceParts[0] == null || nonceParts[1] == null)
            {
                return Tuple.Create(false, false);
            }

            if (!String.Equals(nonce, GenerateNonce(nonceParts[0], ipAddress), StringComparison.Ordinal))
            {
                return Tuple.Create(false, false);
            }

            return !IsNonceStale(nonceParts[0]) ? Tuple.Create(true, false) : Tuple.Create(false, true);
        }

        private bool IsNonceStale(string timestamp)
        {
            double timestampAsDouble;

            if (!Double.TryParse(timestamp, NumberStyles.Float, CultureInfo.InvariantCulture, out timestampAsDouble) || timestampAsDouble <= 0)
            {
                return false;
            }

            DateTime nonceLifetime = DateTime.MinValue.AddMilliseconds(timestampAsDouble);

            return nonceLifetime.Add(NonceLifetime) <= DateTime.UtcNow;
        }
    }
}
