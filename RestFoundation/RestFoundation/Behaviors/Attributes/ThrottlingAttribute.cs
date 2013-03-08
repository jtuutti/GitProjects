// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Behaviors.Attributes
{
    /// <summary>
    /// Represents a bandwidth throttling behavior for a service method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ThrottlingAttribute : ServiceMethodBehaviorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingAttribute"/> class.
        /// </summary>
        /// <param name="delayInMilliseconds">
        /// The number of milliseconds allowed between the calls for a user.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the delay is a non-positive number.
        /// </exception>
        public ThrottlingAttribute(int delayInMilliseconds)
        {
            if (delayInMilliseconds < 1)
            {
                throw new ArgumentOutOfRangeException("delayInMilliseconds");
            }

            DelayInMilliseconds = delayInMilliseconds;
        }

        /// <summary>
        /// Gets the HTTP status code in case of a security exception.
        /// </summary>
        public override HttpStatusCode StatusCode
        {
            get
            {
                return (HttpStatusCode) 429;
            }
        }

        /// <summary>
        /// Gets the HTTP status description in case of a security exception.
        /// </summary>
        public override string StatusDescription
        {
            get
            {
                return Resources.Global.TooManyRequests;
            }
        }

        /// <summary>
        /// Gets the number of milliseconds allowed between the calls for a user.
        /// </summary>
        public int DelayInMilliseconds { get; private set; }

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

            if (serviceContext.Cache == null)
            {
                throw new InvalidOperationException(Resources.Global.UnableToInitializeCache);
            }

            string remoteAddress = serviceContext.Request.ServerVariables.RemoteAddress;

            if (String.IsNullOrEmpty(remoteAddress))
            {
                return BehaviorMethodAction.Execute;
            }

            string cacheKey = String.Concat("throttle-", serviceContext.Request.Url.GetLeftPart(UriPartial.Path), "-", remoteAddress);

            if (serviceContext.Cache.Contains(cacheKey))
            {
                return BehaviorMethodAction.Stop;
            }

            serviceContext.Cache.Add(cacheKey, true, DateTime.Now.AddMilliseconds(DelayInMilliseconds));
            return BehaviorMethodAction.Execute;
        }
    }
}
