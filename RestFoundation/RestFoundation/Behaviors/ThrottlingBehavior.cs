// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a bandwidth throttling behavior for a service.
    /// </summary>
    public class ThrottlingBehavior : SecureServiceBehavior
    {
        private readonly int m_delayInMilliseconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingBehavior"/> class.
        /// </summary>
        /// <param name="delayInMilliseconds">
        /// The number of milliseconds allowed between the calls for a user.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the delay is a non-positive number.
        /// </exception>
        public ThrottlingBehavior(int delayInMilliseconds)
        {
            if (delayInMilliseconds < 1)
            {
                throw new ArgumentOutOfRangeException("delayInMilliseconds");
            }

            m_delayInMilliseconds = delayInMilliseconds;
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
                SetStatus((HttpStatusCode) 429, Resources.Global.TooManyRequests);
                return BehaviorMethodAction.Stop;
            }

            serviceContext.Cache.Add(cacheKey, true, DateTime.Now.AddMilliseconds(m_delayInMilliseconds), CachePriority.Low);
            return BehaviorMethodAction.Execute;
        }
    }
}
