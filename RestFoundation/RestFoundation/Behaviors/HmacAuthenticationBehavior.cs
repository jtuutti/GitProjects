// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using RestFoundation.Resources;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a base class to implement HMAC authentication behavior for a service or a
    /// service method. This class cannot be instantiated.
    /// </summary>
    public abstract class HmacAuthenticationBehavior : SecureServiceBehavior, IAuthenticationBehavior
    {
        private readonly HashAlgorithmType m_algorithmType;

        /// <summary>
        /// Initializes a new instance of the <see cref="HmacAuthenticationBehavior"/> class.
        /// </summary>
        /// <param name="algorithmType">The hash algorithm.</param>
        protected HmacAuthenticationBehavior(HashAlgorithmType algorithmType)
        {
            m_algorithmType = algorithmType;
        }

        /// <summary>
        /// Represents a hash algorithm type.
        /// </summary>
        public enum HashAlgorithmType
        {
            /// <summary>
            /// SHA1 hash algorithm
            /// </summary>
            Sha1,

            /// <summary>
            /// SHA256 hash algorithm
            /// </summary>
            Sha256,

            /// <summary>
            /// SHA384 hash algorithm
            /// </summary>
            Sha384,

            /// <summary>
            /// SHA512 hash algorithm
            /// </summary>
            Sha512,

            /// <summary>
            /// MD5 hash algorithm - Warning: this algorithm is considered insecure and should only be used
            /// for interactions with legacy applications.
            /// </summary>
            [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Md", Justification = "Using standard conventions")]
            Md5
        }

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

            string userId;
            string signature;

            if (!TryGetRequestedSignature(serviceContext.Request, out userId, out signature) ||
                String.IsNullOrWhiteSpace(userId) ||
                String.IsNullOrWhiteSpace(signature))
            {
                serviceContext.Response.SetStatus(HttpStatusCode.Unauthorized, Resources.Global.Unauthorized);
                return BehaviorMethodAction.Stop;
            }

            string hashedServerSignature = HashSignature(userId, GenerateServerSignature(serviceContext), serviceContext.Request);

            return signature == hashedServerSignature ? BehaviorMethodAction.Execute : BehaviorMethodAction.Stop;
        }

        /// <summary>
        /// Hashes and base64 encodes a string signature using the hash algorithm during the class instantiation.
        /// </summary>
        /// <param name="userId">The user id (key or user name).</param>
        /// <param name="signature">The signature.</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The hashed and base64 encoded signature.</returns>
        /// <exception cref="SecurityException">If a user's private key is missing or an invalid hash type is used.</exception>
        protected virtual string HashSignature(string userId, string signature, IHttpRequest request)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            if (signature == null)
            {
                throw new ArgumentNullException("signature");
            }

            using (HashAlgorithm algorithm = CreateAlgorithm(userId, request))
            {
                byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(signature));
                return Convert.ToBase64String(hash);
            }
        }

        /// <summary>
        /// Tries to get the security signature for the current web request. This method should also
        /// be used to validate timestamps or nonces.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="userId">The user id (key or user name).</param>
        /// <param name="signatureHash">The hashed and base64 encoded signature.</param>
        /// <returns>
        /// True if the request signature was found and all the request criteria was satisfied;
        /// otherwise false.
        /// </returns>
        protected abstract bool TryGetRequestedSignature(IHttpRequest request, out string userId, out string signatureHash);

        /// <summary>
        /// Generates the signature for the web request on the server to validate against the user
        /// provided signature.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <returns>The generated signature.</returns>
        protected abstract string GenerateServerSignature(IServiceContext context);

        /// <summary>
        /// Gets the web requests user's private key.
        /// </summary>
        /// <param name="userId">The user id (key or user name).</param>
        /// <param name="request">The HTTP request.</param>
        /// <returns>The private key.</returns>
        protected abstract string GetUserPrivateKey(string userId, IHttpRequest request);

        private HashAlgorithm CreateAlgorithm(string userId, IHttpRequest request)
        {
            string secretKey = GetUserPrivateKey(userId, request);

            if (String.IsNullOrEmpty(secretKey))
            {
                throw new SecurityException(Global.MissingHmacPrivateKey);
            }

            byte[] keyData = Encoding.ASCII.GetBytes(secretKey);

            switch (m_algorithmType)
            {
                case HashAlgorithmType.Sha1:
                    return new HMACSHA1(keyData);
                case HashAlgorithmType.Sha256:
                    return new HMACSHA256(keyData);
                case HashAlgorithmType.Sha384:
                    return new HMACSHA384(keyData);
                case HashAlgorithmType.Sha512:
                    return new HMACSHA512(keyData);
                case HashAlgorithmType.Md5:
                    return new HMACMD5(keyData);
            }

            throw new SecurityException(Global.InvalidHashType);
        }
    }
}
