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
        private readonly IParameterBinder m_parameterBinder;

        public ServiceMethodInvoker(IParameterBinder parameterBinder)
        {
            if (parameterBinder == null) throw new ArgumentNullException("parameterBinder");

            m_parameterBinder = parameterBinder;
        }

        public virtual object Invoke(IRestHandler routeHandler, object service, MethodInfo method)
        {
            if (service == null || method == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "No matching service type or service method was found");
            }

            if (routeHandler == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No route handler was passed to the service method invoker");
            }

            if (routeHandler.Context == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No service context was passed to the service method invoker");
            }

            var behaviorInvoker = new ServiceBehaviorInvoker(routeHandler.Context, service, method);

            List<IServiceBehavior> behaviors = ServiceBehaviorRegistry.GetBehaviors(routeHandler)
                                                                      .Where(behavior => BehaviorAppliesToMethod(behavior, method.Name))
                                                                      .ToList();

            AddServiceBehaviors(behaviors, service, method.Name);

            try
            {
                return InvokeWithBehaviors(routeHandler.Context, behaviorInvoker, behaviors);
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
                    if (InvokeOnExceptionBehaviors(behaviorInvoker, behaviors, internalException))
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

        protected virtual void InvokeOnBindingBehaviors(ServiceBehaviorInvoker serviceBehaviorInvoker, IList<IServiceBehavior> behaviors)
        {
            serviceBehaviorInvoker.PerformOnBindingBehaviors(behaviors.OfType<ISecureServiceBehavior>().ToList());
        }

        protected virtual bool InvokeOnExecutingBehaviors(ServiceBehaviorInvoker serviceBehaviorInvoker, IList<IServiceBehavior> behaviors, object resource)
        {
            return serviceBehaviorInvoker.PerformOnExecutingBehaviors(behaviors, resource);
        }

        protected virtual void InvokeOnExecutedBehaviors(ServiceBehaviorInvoker serviceBehaviorInvoker, IList<IServiceBehavior> behaviors, object result)
        {
            serviceBehaviorInvoker.PerformOnExecutedBehaviors(behaviors, result);
        }

        protected virtual bool InvokeOnExceptionBehaviors(ServiceBehaviorInvoker serviceBehaviorInvoker, IList<IServiceBehavior> behaviors, Exception internalException)
        {
            return serviceBehaviorInvoker.PerformOnExceptionBehaviors(behaviors, internalException);
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

        private object InvokeWithBehaviors(IServiceContext context, ServiceBehaviorInvoker serviceBehaviorInvoker, List<IServiceBehavior> behaviors)
        {
            InvokeOnBindingBehaviors(serviceBehaviorInvoker, behaviors);

            object resource;
            object[] methodArguments = GenerateMethodArguments(context, serviceBehaviorInvoker.Method, out resource);

            if (!InvokeOnExecutingBehaviors(serviceBehaviorInvoker, behaviors, resource))
            {
                return null;
            }

            object result = serviceBehaviorInvoker.Method.Invoke(serviceBehaviorInvoker.Service, methodArguments);
            InvokeOnExecutedBehaviors(serviceBehaviorInvoker, behaviors, result);

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
