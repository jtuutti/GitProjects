// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using RestFoundation.Validation;

namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Represents a resource validation behavior for a service or a service method.
    /// This behavior is set as global service behavior by the default REST configuration.
    /// </summary>
    public class ValidationBehavior : ServiceBehavior
    {
        private readonly IResourceValidator m_validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBehavior"/> class.
        /// </summary>
        public ValidationBehavior() : this(new ResourceValidator())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBehavior"/> class.
        /// </summary>
        /// <param name="validator">The resource validator.</param>
        public ValidationBehavior(IResourceValidator validator)
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
        /// <param name="serviceContext">The service context.</param>
        /// <param name="behaviorContext">The "method executing" behavior context.</param>
        /// <returns>A service method action.</returns>
        public override BehaviorMethodAction OnMethodExecuting(IServiceContext serviceContext, MethodExecutingContext behaviorContext)
        {
            if (serviceContext == null)
            {
                throw new ArgumentNullException("serviceContext");
            }

            if (behaviorContext == null)
            {
                throw new ArgumentNullException("behaviorContext");
            }

            if (behaviorContext.Resource == null || m_validator == null)
            {
                return BehaviorMethodAction.Execute;
            }

            IReadOnlyCollection<ValidationError> validationErrors;

            if (!m_validator.IsValid(behaviorContext.Resource, out validationErrors))
            {
                serviceContext.GetHttpContext().Items[ResourceValidator.ValidationErrorKey] = new ResourceState(validationErrors);
            }

            return BehaviorMethodAction.Execute;
        }
    }
}
