using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a resource validation behavior for a service or a service method.
    /// This behavior is set as global service behavior by the default REST configuration.
    /// </summary>
    public class ResourceValidationBehavior : ServiceBehavior
    {
        private readonly IResourceValidator m_validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceValidationBehavior"/> class.
        /// </summary>
        public ResourceValidationBehavior() : this(new ResourceValidator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceValidationBehavior"/> class.
        /// </summary>
        /// <param name="validator">The resource validator.</param>
        public ResourceValidationBehavior(IResourceValidator validator)
        {
            if (validator == null)
            {
                throw new ArgumentNullException("validator");
            }

            m_validator = validator;
        }

        /// <summary>
        /// Called before a service method is executed.
        /// </summary>
        /// <param name="context">The service context.</param>
        /// <param name="service">The service object.</param>
        /// <param name="method">The service method.</param>
        /// <param name="resource">The resource parameter value, if applicable, or null.</param>
        /// <returns>true to execute the service method; false to stop the request.</returns>
        public override BehaviorMethodAction OnMethodExecuting(IServiceContext context, object service, MethodInfo method, object resource)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (resource == null || m_validator == null)
            {
                return BehaviorMethodAction.Execute;
            }

            ICollection<ValidationError> validationErrors;

            if (!m_validator.IsValid(resource, out validationErrors))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Resource validation failed");
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
