using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;

namespace RestFoundation.Runtime
{
    public class ServiceMethodInvoker : IServiceMethodInvoker
    {
        private readonly ServiceBehaviorInvoker m_behaviorInvoker;
        private readonly IParameterBinder m_parameterBinder;

        public ServiceMethodInvoker(ServiceBehaviorInvoker behaviorInvoker, IParameterBinder parameterBinder)
        {
            if (behaviorInvoker == null) throw new ArgumentNullException("behaviorInvoker");
            if (parameterBinder == null) throw new ArgumentNullException("parameterBinder");

            m_behaviorInvoker = behaviorInvoker;
            m_parameterBinder = parameterBinder;
        }

        public virtual object Invoke(IRestHandler handler, object service, MethodInfo method)
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

            List<IServiceBehavior> behaviors = ServiceBehaviorRegistry.GetBehaviors(handler)
                                                                      .Where(behavior => BehaviorAppliesToMethod(behavior, method.Name))
                                                                      .ToList();

            AddServiceBehaviors(behaviors, service, method.Name);

            try
            {
                return InvokeWithBehaviors(handler.Context, behaviors, service, method);
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
                    if (m_behaviorInvoker.PerformOnExceptionBehaviors(behaviors, service, method, internalException))
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

        private static bool BehaviorAppliesToMethod(IServiceBehavior behavior, string methodName)
        {
            return behavior.AffectedMethods == null || behavior.AffectedMethods.Count == 0 || behavior.AffectedMethods.Contains(methodName);
        }

        private static bool IsWrapperException(Exception ex)
        {
            return (ex is ServiceRuntimeException || ex is TargetInvocationException || ex is AggregateException) && ex.InnerException != null;
        }

        private static void AddServiceBehaviors(List<IServiceBehavior> behaviors, object service, string methodName)
        {
            var restService = service as IRestService;

            if (restService == null || restService.Behaviors == null)
            {
                return;
            }

            foreach (IServiceBehavior serviceBehavior in restService.Behaviors)
            {
                if (BehaviorAppliesToMethod(serviceBehavior, methodName))
                {
                    behaviors.Add(serviceBehavior);
                }
            }
        }

        private object InvokeWithBehaviors(IServiceContext context, List<IServiceBehavior> behaviors, object service, MethodInfo method)
        {
            m_behaviorInvoker.PerformOnBindingBehaviors(behaviors.OfType<ISecureServiceBehavior>().ToList(), service, method);

            object resource;
            object[] methodArguments = GenerateMethodArguments(context, method, out resource);

            if (!m_behaviorInvoker.PerformOnExecutingBehaviors(behaviors, service, method, resource))
            {
                return null;
            }

            object result = method.Invoke(service, methodArguments);
            m_behaviorInvoker.PerformOnExecutedBehaviors(behaviors, service, method, result);

            return result;
        }

        private object[] GenerateMethodArguments(IServiceContext context, MethodInfo method, out object resource)
        {
            var methodArguments = new List<object>();
            resource = null;

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                bool isResource;
                object argumentValue = m_parameterBinder.BindParameter(context, parameter, out isResource);

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
