using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace RestFoundation.Runtime
{
    public class ServiceMethodInvoker : IServiceMethodInvoker
    {
        private readonly IServiceContext m_context;
        private readonly IHttpRequest m_request;
        private readonly IHttpResponse m_response;
        private readonly IParameterBinder m_parameterBinder;

        public ServiceMethodInvoker(IServiceContext context, IHttpRequest request, IHttpResponse response, IParameterBinder parameterBinder)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (request == null) throw new ArgumentNullException("request");
            if (response == null) throw new ArgumentNullException("response");
            if (parameterBinder == null) throw new ArgumentNullException("parameterBinder");

            m_context = context;
            m_request = request;
            m_response = response;
            m_parameterBinder = parameterBinder;
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

            var behaviorInvoker = new BehaviorInvoker(service, method);
            List<IServiceBehavior> behaviors = BehaviorRegistry.GetBehaviors(routeHandler, m_context, m_request, m_response);

            var serviceAsBehavior = service as IServiceBehavior;

            if (serviceAsBehavior != null)
            {
                behaviors.Insert(0, serviceAsBehavior);
            }

            try
            {
                return InvokeWithBehaviors(behaviorInvoker, behaviors);
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
                    if (behaviorInvoker.PerformOnExceptionBehaviors(behaviors, internalException))
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

        public static bool IsWrapperException(Exception ex)
        {
            return (ex is ServiceRuntimeException || ex is TargetInvocationException || ex is AggregateException) && ex.InnerException != null;
        }

        private object InvokeWithBehaviors(BehaviorInvoker behaviorInvoker, List<IServiceBehavior> behaviors)
        {
            behaviorInvoker.PerformOnBindingBehaviors(behaviors.OfType<ISecureServiceBehavior>());

            object resource;
            object[] methodArguments = GenerateMethodArguments(behaviorInvoker.Method, out resource);

            if (!behaviorInvoker.PerformOnExecutingBehaviors(behaviors, resource))
            {
                return null;
            }

            object result = behaviorInvoker.Method.Invoke(behaviorInvoker.Service, methodArguments);
            behaviorInvoker.PerformOnExecutedBehaviors(behaviors, result);

            return result;
        }

        private object[] GenerateMethodArguments(MethodInfo method, out object resource)
        {
            var methodArguments = new List<object>();
            resource = null;

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                bool isResource;
                object argumentValue = m_parameterBinder.BindParameter(parameter, out isResource);

                if (isResource)
                {
                    resource = argumentValue;
                }

                methodArguments.Add(argumentValue);
            }

            return methodArguments.ToArray();
        }
    }
}
