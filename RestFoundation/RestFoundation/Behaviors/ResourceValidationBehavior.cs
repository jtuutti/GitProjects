using System.Collections.Generic;
using System.Net;
using System.Reflection;
using RestFoundation.Runtime;

namespace RestFoundation.Behaviors
{
    public class ResourceValidationBehavior : ServiceBehavior
    {
        private readonly IResourceValidator m_validator;

        public ResourceValidationBehavior()
        {
            m_validator = ObjectActivator.Create<IResourceValidator>();
        }

        public override bool OnMethodExecuting(object service, MethodInfo method, object resource)
        {
            if (resource == null || m_validator == null)
            {
                return true;
            }

            ICollection<ValidationError> validationErrors;

            if (!m_validator.IsValid(resource, out validationErrors))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, "Resource validation failed");
            }

            return true;
        }
    }
}
