﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a service method that returns performs an OData operation and returns
    /// the specified maximum number of results, if the $top value has not been provided.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class MaxQueryResultsAttribute : ServiceMethodBehaviorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxQueryResultsAttribute"/> class.
        /// </summary>
        /// <param name="maxResults">The maximum number of results to return.</param>
        public MaxQueryResultsAttribute(int maxResults)
        {
            if (maxResults <= 0)
            {
                throw new ArgumentOutOfRangeException("maxResults");
            }

            MaxResults = maxResults;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the number of maximum results can be overridden
        /// by providing a $take value in the query string.
        /// </summary>
        public bool AllowOverride { get; set; }

        /// <summary>
        /// Gets the maximum number of results to return.
        /// </summary>
        public int MaxResults { get; private set; }

        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodExecuting(IServiceContext serviceContext, MethodExecutingContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            serviceContext.GetHttpContext().Items[ServiceCallConstants.MaxQueryResults] = AllowOverride ? (MaxResults * -1) : MaxResults;

            return BehaviorMethodAction.Execute;
        }
    }
}
