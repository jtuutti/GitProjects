using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using RestFoundation.DataFormatters;

namespace RestFoundation.Runtime
{
    public class ServiceMethodInvoker : IServiceMethodInvoker
    {
        protected const string ResourceParameterName = "resource";

        private readonly IServiceContext m_context;
        private readonly IHttpRequest m_request;
        private readonly IHttpResponse m_response;

        public ServiceMethodInvoker(IServiceContext context, IHttpRequest request, IHttpResponse response)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");

            m_context = context;
            m_request = request;
            m_response = response;
        }

        public virtual object Invoke(IRouteHandler routeHandler, object service, MethodInfo method)
        {
            if (service == null || method == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "No matching service type or service method was found");
            }

            if (routeHandler == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route handler was passed to the service method invoker");
            }

            List<IServiceBehavior> behaviors = BehaviorRegistry.GetBehaviors(routeHandler, m_context, m_request, m_response);

            var serviceAsBehavior = service as IServiceBehavior;

            if (serviceAsBehavior != null)
            {
                behaviors.Insert(0, serviceAsBehavior);
            }

            try
            {
                return InvokeWithBehaviors(service, method, behaviors);
            }
            catch (Exception ex)
            {
                Exception internalException = IsWrapperException(ex) ? ex.InnerException : ex;

                if (internalException is HttpResponseException || internalException is HttpRequestValidationException)
                {
                    if (ex == internalException)
                    {
                        throw;
                    }

                    throw internalException;
                }

                try
                {
                    if (PerformOnExceptionBehaviors(behaviors, service, method, internalException))
                    {
                        throw new ServiceRuntimeException(internalException);
                    }
                }
                catch (Exception innerEx)
                {
                    if (IsWrapperException(innerEx))
                    {
                        throw new ServiceRuntimeException(innerEx.InnerException, ex);
                    }

                    throw new ServiceRuntimeException(innerEx, ex);
                }
            }

            return null;
        }

        private static void PerformOnBindingBehaviors(IEnumerable<ISecureServiceBehavior> behaviors, object service, MethodInfo method)
        {
            foreach (ISecureServiceBehavior behavior in behaviors)
            {
                behavior.OnMethodAuthorizing(service, method);
            }
        }

        private static bool PerformOnExecutingBehaviors(List<IServiceBehavior> behaviors, object service, MethodInfo method, object resource)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodExecuting(service, method, resource))
                {
                    return false;
                }
            }

            return true;
        }

        private static void PerformOnExecutedBehaviors(List<IServiceBehavior> behaviors, object service, MethodInfo method, object result)
        {
            for (int i = behaviors.Count - 1; i >= 0; i--)
            {
                behaviors[i].OnMethodExecuted(service, method, result);
            }
        }

        private static bool PerformOnExceptionBehaviors(List<IServiceBehavior> behaviors, object service, MethodInfo method, Exception ex)
        {
            for (int i = 0; i < behaviors.Count; i++)
            {
                if (!behaviors[i].OnMethodException(service, method, ex))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsWrapperException(Exception ex)
        {
            return (ex is ServiceRuntimeException || ex is TargetInvocationException || ex is AggregateException) && ex.InnerException != null;
        }

        private object[] GenerateMethodArguments(MethodInfo method, out object resource)
        {
            var methodArguments = new List<object>();

            resource = null;

            foreach (ParameterInfo parameter in method.GetParameters())
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

        private object InvokeWithBehaviors(object service, MethodInfo method, List<IServiceBehavior> behaviors)
        {
            PerformOnBindingBehaviors(behaviors.OfType<ISecureServiceBehavior>(), service, method);

            object resource;
            object[] methodArguments = GenerateMethodArguments(method, out resource);

            if (!PerformOnExecutingBehaviors(behaviors, service, method, resource))
            {
                return null;
            }

            object result = method.Invoke(service, methodArguments);

            PerformOnExecutedBehaviors(behaviors, service, method, result);

            return result;
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
