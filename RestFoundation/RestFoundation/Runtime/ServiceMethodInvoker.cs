// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using RestFoundation.Behaviors;
using RestFoundation.Results;
using RestFoundation.Runtime.Handlers;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents a service method invoker.
    /// </summary>
    public class ServiceMethodInvoker : IServiceMethodInvoker
    {
        private static readonly ConcurrentDictionary<MethodInfo, List<ServiceMethodBehaviorAttribute>> methodBehaviors = new ConcurrentDictionary<MethodInfo, List<ServiceMethodBehaviorAttribute>>();

        private readonly IServiceBehaviorInvoker m_behaviorInvoker;
        private readonly IParameterValueProvider m_parameterValueProvider;
        private readonly IResultFactory m_resultFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMethodInvoker"/> class.
        /// </summary>
        /// <param name="behaviorInvoker">The service behavior invoker.</param>
        /// <param name="parameterValueProvider">The parameter value provider.</param>
        /// <param name="resultFactory">The service method result factory.</param>
        public ServiceMethodInvoker(IServiceBehaviorInvoker behaviorInvoker, IParameterValueProvider parameterValueProvider, IResultFactory resultFactory)
        {
            if (behaviorInvoker == null)
            {
                throw new ArgumentNullException("behaviorInvoker");
            }

            if (parameterValueProvider == null)
            {
                throw new ArgumentNullException("parameterValueProvider");
            }

            if (resultFactory == null)
            {
                throw new ArgumentNullException("resultFactory");
            }

            m_behaviorInvoker = behaviorInvoker;
            m_parameterValueProvider = parameterValueProvider;
            m_resultFactory = resultFactory;
        }

        /// <summary>
        /// Invokes the service method.
        /// </summary>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <param name="token">The cancellation token for the returned task.</param>
        /// <returns>A task that invokes the service method.</returns>
        public virtual Task Invoke(object service, MethodInfo method, IRestServiceHandler handler, CancellationToken token)
        {
            if (service == null || method == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, RestResources.MismatchedServiceMethod);
            }

            if (handler == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.MissingRouteHandler);
            }

            if (handler.Context == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, RestResources.MissingServiceContext);
            }

            var invocationTask = new Task(ctx =>
            {
                TrySetHttpContext(ctx);

                List<IServiceBehavior> behaviors = GetServiceMethodBehaviors(service, method, handler);
                AddServiceBehaviors(method, behaviors);

                LogUtility.LogRequestData(handler.Context.GetHttpContext());

                object returnedObject = InvokeAndProcessExceptions(service, method, behaviors, handler);
                CreateResultTask(returnedObject, method, handler);

                LogUtility.LogResponseData(handler.Context.GetHttpContext());
            }, HttpContext.Current, token);

            return invocationTask;
        }

        private static void TrySetHttpContext(object context)
        {
            if (context != null && HttpContext.Current == null)
            {
                HttpContext.Current = (HttpContext) context;
            }
        }

        private static List<IServiceBehavior> GetServiceMethodBehaviors(object service, MethodInfo method, IRestServiceHandler handler)
        {
            List<IServiceBehavior> behaviors = ServiceBehaviorRegistry.GetBehaviors(handler)
                                                                      .Where(behavior => behavior.AppliesTo(handler.Context, new MethodAppliesContext(service, method)))
                                                                      .ToList();

            return behaviors;
        }

        private static void AddServiceBehaviors(MethodInfo method, List<IServiceBehavior> behaviors)
        {
            List<ServiceMethodBehaviorAttribute> methodBehaviorList = methodBehaviors.GetOrAdd(method, m =>
            {
                var methodBehaviorAttributes = m.GetCustomAttributes(typeof(ServiceMethodBehaviorAttribute), false)
                                                .Cast<ServiceMethodBehaviorAttribute>()
                                                .OrderBy(b => b.Order)
                                                .ToList();

                if (LogUtility.CanLog)
                {
                    foreach (ServiceMethodBehaviorAttribute methodBehaviorAttribute in methodBehaviorAttributes)
                    {
                        Type behaviorAttributeType = methodBehaviorAttribute.GetType();

                        if (!behaviorAttributeType.IsSealed)
                        {
                            LogUtility.LogUnsealedBehaviorAttribute(behaviorAttributeType);
                        }
                    }
                }

                return methodBehaviorAttributes;
            });

            foreach (ServiceMethodBehaviorAttribute methodBehavior in methodBehaviorList)
            {
                behaviors.Add(methodBehavior);
            }
        }

        private object InvokeAndProcessExceptions(object service, MethodInfo method, List<IServiceBehavior> behaviors, IRestServiceHandler handler)
        {
            try
            {
                return InvokeWithBehaviors(service, method, behaviors, handler);
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
                    IServiceExceptionHandler exceptionHandler = ServiceExceptionHandlerRegistry.GetExceptionHandler(handler);

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

        private object InvokeWithBehaviors(object service, MethodInfo method, List<IServiceBehavior> behaviors, IRestServiceHandler handler)
        {
            m_behaviorInvoker.InvokeOnAuthorizingBehaviors(behaviors.OfType<ISecureServiceBehavior>().ToList(), service, method);

            object resource;
            object[] methodArguments = GenerateMethodArguments(method, handler, out resource);

            if (!ReferenceEquals(null, resource))
            {
                handler.Context.Request.ResourceBag.Resource = resource;
            }

            if (m_behaviorInvoker.InvokeOnExecutingBehaviors(behaviors, service, method, resource) == BehaviorMethodAction.Stop)
            {
                return null;
            }

            object result = method.Invoke(service, methodArguments);
            m_behaviorInvoker.InvokeOnExecutedBehaviors(behaviors, service, method, result);

            return result;
        }

        private object[] GenerateMethodArguments(MethodInfo method, IRestServiceHandler handler, out object resource)
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

        private void CreateResultTask(object returnedObject, MethodInfo method, IRestServiceHandler handler)
        {
            var resultInvoker = new ResultExecutor();

            if (method.ReturnType == typeof(void))
            {
                resultInvoker.ExecuteNoContent(handler.Context);
            }

            IResult result = m_resultFactory.Create(returnedObject, method.ReturnType, handler);

            resultInvoker.Execute(result, handler.Context);
        }
    }
}
