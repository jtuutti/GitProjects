using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using RestFoundation.Behaviors;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a service method invoker.
    /// </summary>
    public class ServiceMethodInvoker : IServiceMethodInvoker
    {
        private readonly IServiceBehaviorInvoker m_behaviorInvoker;
        private readonly IParameterValueProvider m_parameterValueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMethodInvoker"/> class.
        /// </summary>
        /// <param name="behaviorInvoker">The service behavior invoker.</param>
        /// <param name="parameterValueProvider">The parameter value provider.</param>
        public ServiceMethodInvoker(IServiceBehaviorInvoker behaviorInvoker, IParameterValueProvider parameterValueProvider)
        {
            if (behaviorInvoker == null)
            {
                throw new ArgumentNullException("behaviorInvoker");
            }

            if (parameterValueProvider == null)
            {
                throw new ArgumentNullException("parameterValueProvider");
            }

            m_behaviorInvoker = behaviorInvoker;
            m_parameterValueProvider = parameterValueProvider;
        }

        /// <summary>
        /// Invokes the service method.
        /// </summary>
        /// <param name="method">The service method.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <returns>The return value of the executed service method.</returns>
        public virtual object Invoke(MethodInfo method, object service, IRestHandler handler)
        {
            if (service == null || method == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "No matching service type or service method was found");
            }

            if (handler == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route handler was passed to the service method invoker");
            }

            if (handler.Context == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No service context was passed to the service method invoker");
            }

            List<IServiceBehavior> behaviors = GetServiceMethodBehaviors(handler, method, service);
            AddServiceBehaviors(method, service, behaviors);

            return InvokeAndProcessExceptions(method, service, behaviors, handler);
        }

        private static List<IServiceBehavior> GetServiceMethodBehaviors(IRestHandler handler, MethodInfo method, object service)
        {
            List<IServiceBehavior> behaviors = ServiceBehaviorRegistry.GetBehaviors(handler)
                                                                      .Where(behavior => behavior.AppliesTo(service.GetType(), method))
                                                                      .ToList();

            return behaviors;
        }

        private static void AddServiceBehaviors(MethodInfo method, object service, List<IServiceBehavior> behaviors)
        {
            var restService = service as IRestService;

            if (restService == null || restService.Behaviors == null)
            {
                return;
            }

            foreach (IServiceBehavior serviceBehavior in restService.Behaviors)
            {
                if (serviceBehavior.AppliesTo(service.GetType(), method))
                {
                    behaviors.Add(serviceBehavior);
                }
            }
        }

        private static IServiceExceptionHandler GetServiceExceptionHandler(object service, IRestHandler handler)
        {
            IServiceExceptionHandler exceptionHandler;

            var restService = service as IRestService;

            if (restService != null && restService.ExceptionHandler != null)
            {
                exceptionHandler = restService.ExceptionHandler;
            }
            else
            {
                exceptionHandler = ServiceExceptionHandlerRegistry.GetExceptionHandler(handler);
            }

            return exceptionHandler;
        }

        private object InvokeAndProcessExceptions(MethodInfo method, object service, List<IServiceBehavior> behaviors, IRestHandler handler)
        {
            try
            {
                return InvokeWithBehaviors(method, service, behaviors, handler);
            }
            catch (Exception ex)
            {
                Exception unwrappedException = ExceptionUnwrapper.Unwrap(ex);

                if (ExceptionUnwrapper.IsDirectResponseException(unwrappedException))
                {
                    if (ex == unwrappedException)
                    {
                        throw;
                    }

                    throw unwrappedException;
                }

                try
                {
                    IServiceExceptionHandler exceptionHandler = GetServiceExceptionHandler(service, handler);

                    if (exceptionHandler != null && exceptionHandler.Handle(handler.Context, service, method, unwrappedException) == ExceptionAction.Handle)
                    {
                        return null;
                    }

                    throw new ServiceRuntimeException(unwrappedException);
                }
                catch (ServiceRuntimeException)
                {
                    throw;
                }
                catch (Exception innerEx)
                {
                    throw new ServiceRuntimeException(ExceptionUnwrapper.Unwrap(innerEx), ex);
                }
            }
        }

        private object InvokeWithBehaviors(MethodInfo method, object service, List<IServiceBehavior> behaviors, IRestHandler handler)
        {
            m_behaviorInvoker.InvokeOnAuthorizingBehaviors(behaviors.OfType<ISecureServiceBehavior>().ToList(), service, method);

            object resource;
            object[] methodArguments = GenerateMethodArguments(method, handler, out resource);

            if (m_behaviorInvoker.InvokeOnExecutingBehaviors(behaviors, service, method, resource) == BehaviorMethodAction.Stop)
            {
                return null;
            }

            object result = method.Invoke(service, methodArguments);
            m_behaviorInvoker.InvokeOnExecutedBehaviors(behaviors, service, method, result);

            return result;
        }

        private object[] GenerateMethodArguments(MethodInfo method, IRestHandler handler, out object resource)
        {
            var methodArguments = new List<object>();
            resource = null;

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                bool isResource;
                object argumentValue = m_parameterValueProvider.CreateValue(parameter, handler, out isResource);

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
