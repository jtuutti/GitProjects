// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Globalization;
using System.Net;
using System.Security.Principal;
using System.Text;
using RestFoundation.Runtime;
using RestFoundation.Security;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a basic authentication secure behavior for a service or a service method.
    /// </summary>
    public class DigestAuthenticationBehavior : SecureServiceBehavior, IAuthenticationBehavior, IDisposable
    {
        private const string AuthenticationType = "Digest";
        private const int DefaultNonceLifeTimeInSeconds = 300;

        private readonly IAuthorizationManager m_authorizationManager;
        private readonly MD5Encoder m_encoder;
        private readonly RijndaelEncryptor m_encryptor;

        private bool m_isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigestAuthenticationBehavior"/> class.
        /// </summary>
        public DigestAuthenticationBehavior() : this(Rest.Configuration.ServiceLocator.GetService<IAuthorizationManager>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigestAuthenticationBehavior"/> class.
        /// </summary>
        /// <param name="authorizationManager">The authorization manager.</param>
        public DigestAuthenticationBehavior(IAuthorizationManager authorizationManager)
        {
            if (authorizationManager == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, Resources.Global.MissingAuthorizationManager);
            }

            NonceLifetime = TimeSpan.FromSeconds(DefaultNonceLifeTimeInSeconds);
            Qop = QualityOfProtection.Auth;

            m_authorizationManager = authorizationManager;
            m_encoder = new MD5Encoder();
            m_encryptor = new RijndaelEncryptor();
        }

        /// <summary>
        /// Contains the quality of protection values for digest authentication.
        /// </summary>
        public enum QualityOfProtection
        {
            /// <summary>
            /// None
            /// </summary>
            None,

            /// <summary>
            /// Auth
            /// </summary>
            Auth
        }

        /// <summary>
        /// Gets or sets the nonce lifetime.
        /// </summary>
        public TimeSpan NonceLifetime { get; set; }

        /// <summary>
        /// Gets or sets the quality of protection value.
        /// </summary>
        public QualityOfProtection Qop { get; set; }

        /// <summary>
        /// Called during the authorization process before a service method or behavior is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method authorizing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodAuthorizing(IServiceContext serviceContext, MethodAuthorizingContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            AuthorizationHeader header;

            if (!AuthorizationHeaderParser.TryParse(serviceContext.Request.Headers.TryGet("Authorization"), serviceContext.Request.Headers.ContentCharsetEncoding, out header) ||
                !AuthenticationType.Equals(header.AuthenticationType, StringComparison.OrdinalIgnoreCase))
            {
                GenerateAuthenticationHeader(serviceContext, false);
                return BehaviorMethodAction.Stop;
            }

            Credentials credentials = m_authorizationManager.GetCredentials(header.UserName);

            if (credentials == null)
            {
                GenerateAuthenticationHeader(serviceContext, false);
                return BehaviorMethodAction.Stop;
            }

            Tuple<bool, bool> responseValidationResult = ValidateResponse(serviceContext, header, credentials);

            if (!responseValidationResult.Item1)
            {
                GenerateAuthenticationHeader(serviceContext, responseValidationResult.Item2);
                return BehaviorMethodAction.Stop;
            }

            serviceContext.User = new GenericPrincipal(new GenericIdentity(header.UserName, AuthenticationType), credentials.GetRoles());
            return BehaviorMethodAction.Execute;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DigestAuthenticationBehavior"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (m_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                m_encoder.Dispose();
            }

            m_isDisposed = true;
        }

        private string GenerateNonce(IServiceContext serviceContext, string timestamp)
        {
            if (String.IsNullOrWhiteSpace(serviceContext.Request.ServerVariables.RemoteAddress) ||
                String.IsNullOrWhiteSpace(serviceContext.Request.ServerVariables.LocalAddress))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, Resources.Global.Forbidden);
            }

            string nonce = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture,
                                                          "{0}:{1}:{2}",
                                                          serviceContext.Request.ServerVariables.RemoteAddress,
                                                          serviceContext.Request.ServerVariables.LocalAddress,
                                                          serviceContext.Request.ServerVariables.ServerPort));

            return m_encryptor.Encrypt(String.Format(CultureInfo.InvariantCulture, "{0}:{1}", timestamp, nonce));
        }

        private void GenerateAuthenticationHeader(IServiceContext serviceContext, bool isStale)
        {
            string timestamp = (DateTime.UtcNow - DateTime.MinValue).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);

            var headerBuilder = new StringBuilder();
            headerBuilder.Append(AuthenticationType).Append(' ');
            headerBuilder.AppendFormat(CultureInfo.InvariantCulture, "realm=\"{0}\"", serviceContext.Request.Url.OperationUrl);
            headerBuilder.AppendFormat(CultureInfo.InvariantCulture, ", nonce=\"{0}\"", GenerateNonce(serviceContext, timestamp));

            if (Qop == QualityOfProtection.Auth)
            {
                headerBuilder.AppendFormat(CultureInfo.InvariantCulture, ", qop=\"auth\"");
            }

            if (isStale)
            {
                headerBuilder.AppendFormat(CultureInfo.InvariantCulture, ", stale=\"TRUE\"");
            }

            serviceContext.Response.Output.Clear();
            serviceContext.Response.SetHeader("WWW-Authenticate", headerBuilder.ToString());
            serviceContext.Response.SetStatus(HttpStatusCode.Unauthorized, Resources.Global.Unauthorized);
        }

        private Tuple<bool, bool> ValidateResponse(IServiceContext serviceContext, AuthorizationHeader header, Credentials credentials)
        {
            if (String.IsNullOrWhiteSpace(header.UserName) || header.Parameters == null)
            {
                return Tuple.Create(false, false);
            }

            string ipAddress = serviceContext.Request.ServerVariables.RemoteAddress;

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

            if (String.IsNullOrEmpty(credentials.Password))
            {
                return Tuple.Create(false, false);
            }

            string nonce = header.Parameters.Get("nonce");           
            Tuple<bool, bool> nonceValidationResult = ValidateNonce(serviceContext, nonce);

            if (!nonceValidationResult.Item1)
            {
                return nonceValidationResult;
            }

            string ha1 = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}:{2}", header.UserName, realm, credentials.Password));
            string ha2 = m_encoder.Encode(String.Format(CultureInfo.InvariantCulture, "{0}:{1}", serviceContext.Request.Method.ToString().ToUpperInvariant(), uri));

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

        private Tuple<bool, bool> ValidateNonce(IServiceContext serviceContext, string nonce)
        {
            if (String.IsNullOrEmpty(nonce))
            {
                return Tuple.Create(false, false);
            }

            string nonceString;

            try
            {
                nonceString = m_encryptor.Decrypt(nonce);
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

            if (!String.Equals(nonce, GenerateNonce(serviceContext, nonceParts[0]), StringComparison.Ordinal))
            {
                return Tuple.Create(false, false);
            }

            return !IsNonceStale(nonceParts[0]) ? Tuple.Create(true, false) : Tuple.Create(false, true);
        }

        private bool IsNonceStale(string timestamp)
        {
            if (NonceLifetime.TotalSeconds <= 0)
            {
                return false;
            }

            double timestampAsDouble;

            if (!Double.TryParse(timestamp, NumberStyles.Float, CultureInfo.InvariantCulture, out timestampAsDouble) || timestampAsDouble <= 0)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, Resources.Global.Forbidden);
            }

            DateTime nonceTimeStamp = DateTime.MinValue.AddMilliseconds(timestampAsDouble);

            return nonceTimeStamp.Add(NonceLifetime) <= DateTime.UtcNow;
        }
    }
}
