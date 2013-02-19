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
        private readonly IResultWrapper m_resultWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMethodInvoker"/> class.
        /// </summary>
        /// <param name="behaviorInvoker">The service behavior invoker.</param>
        /// <param name="parameterValueProvider">The parameter value provider.</param>
        /// <param name="resultWrapper">The service method result wrapper.</param>
        public ServiceMethodInvoker(IServiceBehaviorInvoker behaviorInvoker, IParameterValueProvider parameterValueProvider, IResultWrapper resultWrapper)
        {
            if (behaviorInvoker == null)
            {
                throw new ArgumentNullException("behaviorInvoker");
            }

            if (parameterValueProvider == null)
            {
                throw new ArgumentNullException("parameterValueProvider");
            }

            if (resultWrapper == null)
            {
                throw new ArgumentNullException("resultWrapper");
            }

            m_behaviorInvoker = behaviorInvoker;
            m_parameterValueProvider = parameterValueProvider;
            m_resultWrapper = resultWrapper;
        }

        /// <summary>
        /// Invokes the service method.
        /// </summary>
        /// <param name="handler">The REST handler associated with the HTTP request.</param>
        /// <param name="service">The service instance.</param>
        /// <param name="method">The service method.</param>
        /// <param name="token">The cancellation token for the returned task.</param>
        /// <returns>A task that invokes the service method.</returns>
        public virtual async Task Invoke(IRestServiceHandler handler, object service, MethodInfo method, CancellationToken token)
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

            List<IServiceBehavior> behaviors = GetServiceMethodBehaviors(handler, service, method);
            AddServiceBehaviors(method, behaviors);
            token.ThrowIfCancellationRequested();

            LogUtility.LogRequestData(handler.Context.GetHttpContext());

            object returnedObject = InvokeAndProcessExceptions(handler, service, method, behaviors);
            token.ThrowIfCancellationRequested();

            await CreateResultTask(handler, returnedObject, method.ReturnType);
            token.ThrowIfCancellationRequested();

            LogUtility.LogResponseData(handler.Context.GetHttpContext());
        }

        private static List<IServiceBehavior> GetServiceMethodBehaviors(IRestServiceHandler handler, object service, MethodInfo method)
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

        private static Tuple<object, Type> GetTaskInfo(Task task)
        {
            Type taskType = task.GetType();

            bool isVoidTask = taskType == typeof(Task);
            bool isTypedTask = taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>);

            if (!isVoidTask && !isTypedTask)
            {
                throw new InvalidOperationException(RestResources.InvalidIAsyncResultReturned);
            }

            if (isVoidTask)
            {
                return new Tuple<object, Type>(null, typeof(void));
            }

            Type taskResultType = taskType.GetGenericArguments()[0];
            var dynamicTask = (dynamic) task;

            return new Tuple<object, Type>(dynamicTask.Result, taskResultType);
        }

        private object InvokeAndProcessExceptions(IRestServiceHandler handler, object service, MethodInfo method, List<IServiceBehavior> behaviors)
        {
            try
            {
                return InvokeWithBehaviors(handler, service, method, behaviors);
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

        private object InvokeWithBehaviors(IRestServiceHandler handler, object service, MethodInfo method, List<IServiceBehavior> behaviors)
        {
            m_behaviorInvoker.InvokeOnAuthorizingBehaviors(behaviors.OfType<ISecureServiceBehavior>().ToList(), service, method);

            object resource;
            object[] methodArguments = GenerateMethodArguments(handler, method, out resource);

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

        private object[] GenerateMethodArguments(IRestServiceHandler handler, MethodInfo method, out object resource)
        {
            var methodArguments = new List<object>();
            resource = null;

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                bool isResource;
                object argumentValue = m_parameterValueProvider.CreateValue(handler, parameter, out isResource);

                if (isResource)
                {
                    resource = argumentValue;
                }

                methodArguments.Add(argumentValue);
            }

            return methodArguments.ToArray();
        }

        private async Task CreateResultTask(IRestServiceHandler handler, object returnedObject, Type methodReturnType)
        {
            var returnedTask = returnedObject as Task;

            if (returnedTask != null)
            {
                await returnedTask;

                var taskInfo = GetTaskInfo(returnedTask);
                returnedObject = taskInfo.Item1;
                methodReturnType = taskInfo.Item2;
            }

            if (returnedObject is IAsyncResult)
            {
                throw new InvalidOperationException(RestResources.InvalidIAsyncResultReturned);
            }

            var resultExecutor = new ResultExecutor();

            if (methodReturnType == typeof(void))
            {
                resultExecutor.ExecuteNoContent(handler.Context);
                return;
            }

            IResult result = m_resultWrapper.Wrap(handler, returnedObject, methodReturnType);
            resultExecutor.Execute(result, handler.Context);
        }
    }
}
