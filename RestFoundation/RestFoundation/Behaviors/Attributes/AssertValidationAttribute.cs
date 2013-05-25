// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors.Attributes
{
    /// <summary>
    /// Represents a service method that creates and returns a fault collection if the resource validation
    /// failed before the method execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AssertValidationAttribute : ServiceMethodBehaviorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertValidationAttribute"/> class.
        /// </summary>
        public AssertValidationAttribute() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertValidationAttribute"/> class.
        /// </summary>
        /// <param name="throwOnNullResource">
        /// A value indicating whether there should be a fault created if the resource is null.
        /// </param>
        public AssertValidationAttribute(bool throwOnNullResource)
        {
            ThrowOnNullResource = throwOnNullResource;
        }

        /// <summary>
        /// Gets a value indicating whether a null resource is allowed.
        /// </summary>
        public bool ThrowOnNullResource { get; private set; }

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

            if (serviceContext.Request.Method != HttpMethod.Post && serviceContext.Request.Method != HttpMethod.Put && serviceContext.Request.Method != HttpMethod.Patch)
            {
                return base.OnMethodExecuting(serviceContext, behaviorContext);
            }

            if (ThrowOnNullResource && ReferenceEquals(null, serviceContext.Request.ResourceBag.Resource))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, Resources.Global.NoResourceProvided);
            }

            if (!serviceContext.Request.ResourceState.IsValid)
            {
                throw new HttpResourceFaultException();
            }

            return base.OnMethodExecuting(serviceContext, behaviorContext);
        }
    }
}
