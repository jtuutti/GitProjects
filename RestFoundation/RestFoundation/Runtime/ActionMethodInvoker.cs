using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using RestFoundation.DataFormatters;

namespace RestFoundation.Runtime
{
    public class ActionMethodInvoker : IActionMethodInvoker
    {
        protected const string ResourceParameterName = "resource";

        private readonly IHttpRequest m_request;
        private readonly IHttpResponse m_response;

        public ActionMethodInvoker(IHttpRequest request, IHttpResponse response)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            m_request = request;
            m_response = response;
        }

        public virtual object Invoke(IRouteHandler routeHandler, object service, MethodInfo actionMethod)
        {
            if (service == null || actionMethod == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "No matching service type or action method was found");
            }

            if (routeHandler == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route handler was passed to the action invoker");
            }

            List<IServiceBehavior> behaviors = BehaviorRegistry.GetBehaviors(routeHandler, m_request, m_response);
            var serviceBehavior = service as IServiceBehavior;

            if (serviceBehavior != null)
            {
                serviceBehavior.OnActionBinding(serviceBehavior, actionMethod);
            }

            PerformOnBindingBehaviors(behaviors, service, actionMethod);

            object resource;
            object[] methodArguments = GenerateMethodArguments(actionMethod, out resource);

            if (serviceBehavior != null)
            {
                if (!serviceBehavior.OnActionExecuting(resource))
                {
                    return null;
                }
            }

            if (!PerformOnExecutingBehaviors(behaviors, resource))
            {
                return null;
            }

            object result;

            try
            {
                result = actionMethod.Invoke(service, methodArguments);   
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is HttpResponseException || ex.InnerException is HttpRequestValidationException)
                {
                    throw ex.InnerException;
                }

                throw;
            }

            PerformOnExecutedBehaviors(behaviors, result);

            if (serviceBehavior != null)
            {
                serviceBehavior.OnActionExecuted(result);
            }

            return result;
        }

        private static void PerformOnBindingBehaviors(List<IServiceBehavior> behaviors, object service, MethodInfo actionMethod)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                behaviors[i].OnActionBinding(service, actionMethod);
            }
        }

        private static bool PerformOnExecutingBehaviors(List<IServiceBehavior> behaviors, object resource)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnActionExecuting(resource))
                {
                    return false;
                }
            }

            return true;
        }

        private static void PerformOnExecutedBehaviors(List<IServiceBehavior> behaviors, object result)
        {
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnActionExecuted(result);
            }
        }

        private object[] GenerateMethodArguments(MethodInfo actionMethod, out object resource)
        {
            var methodArguments = new List<object>();

            resource = null;

            foreach (ParameterInfo parameter in actionMethod.GetParameters())
            {
                object parameterRoute = m_request.RouteValues.TryGet(parameter.Name);

                if (parameterRoute != null)
                {
                    object argumentValue = SafeConvert.ChangeType(parameterRoute, parameter.ParameterType);
                    methodArguments.Add(argumentValue);
                    continue;
                }

                if (String.Equals(ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase))
                {
                    resource = GetResource(parameter);
                    methodArguments.Add(resource);
                    continue;
                }

                methodArguments.Add(null);
            }

            return methodArguments.ToArray();
        }

        private object GetResource(ParameterInfo parameter)
        {
            IDataFormatter formatter = Formatters.GetFormatter(m_request.Headers.ContentType);

            object argumentValue;

            try
            {
                argumentValue = formatter.Format(m_request.Body, m_request.Headers.ContentCharsetEncoding, parameter.ParameterType);
            }
            catch (Exception ex)
            {
                // Log the original exception {ex}

                if (ex is HttpRequestValidationException)
                {
                    throw;
                }

                throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid resource body provided");
            }

            return argumentValue;
        }
    }
}
