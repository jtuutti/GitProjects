using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace MvcAlt.Infrastructure
{
    public class DefaultActionMethodResolver : IActionMethodResolver
    {
        private static readonly ConcurrentDictionary<ActionRoute, Delegate> actionDelegates = new ConcurrentDictionary<ActionRoute, Delegate>();

        public Delegate Resolve(IHttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (!request.RouteValues.ContainsKey("_controller") || !request.RouteValues.ContainsKey("_method"))
            {
                throw new HttpException(404, "The current route cannot be resolved to an action method");
            }

            string controllerName = request.RouteValues["_controller"].ToString();
            string methodName = request.RouteValues["_method"].ToString();

            if (String.IsNullOrWhiteSpace(controllerName) || String.IsNullOrWhiteSpace(methodName))
            {
                throw new HttpException(404, "The current route cannot be resolved to an action method");
            }

            var route = new ActionRoute(controllerName, methodName, request.Verb);

            return actionDelegates.GetOrAdd(route, d => CreateDelegate(controllerName, methodName));
        }

        private static Delegate CreateDelegate(string controllerName, string methodName)
        {
            Type controllerType = GetControllerType(controllerName);

            if (controllerType == null)
            {
                throw new HttpException(404, "The current route cannot be resolved to an action method");
            }

            MethodInfo method;

            try
            {
                method = controllerType.GetMethod(methodName);
            }
            catch (AmbiguousMatchException ex)
            {
                throw new HttpException(500, "The current route resolves to more than one action method", ex);
            }

            if (method == null)
            {
                throw new HttpException(404, "The current route cannot be resolved to an action method");
            }

            if (method.GetParameters().Length > 1)
            {
                throw new HttpException(500, "Action methods with multiple input parameters are not supported");
            }

            var args = new List<Type>(method.GetParameters().Select(p => p.ParameterType));

            Type delegateType;

            if (method.ReturnType == typeof(void))
            {
                delegateType = Expression.GetActionType(args.ToArray());
            }
            else
            {
                args.Add(method.ReturnType);
                delegateType = Expression.GetFuncType(args.ToArray());
            }

            return Delegate.CreateDelegate(delegateType, null, method);
        }

        private static Type GetControllerType(string controllerName)
        {
            return MvcConfiguration.Current.ControllerAssembly.GetTypes().FirstOrDefault(t => t.Name == controllerName &&
                                                                                              typeof(IController).IsAssignableFrom(t));
        }

        private struct ActionRoute
        {
            private readonly string controllerName;
            private readonly string methodName;
            private readonly HttpVerb verb;

            public ActionRoute(string controllerName, string methodName, HttpVerb verb)
            {
                if (String.IsNullOrEmpty(controllerName)) throw new ArgumentNullException("controllerName");
                if (String.IsNullOrEmpty(methodName)) throw new ArgumentNullException("methodName");

                this.controllerName = controllerName;
                this.methodName = methodName;
                this.verb = verb;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj) || !(obj is ActionRoute))
                {
                    return false;
                }

                var other = (ActionRoute) obj;

                return Equals(other.controllerName, controllerName) && Equals(other.methodName, methodName) && Equals(other.verb, verb);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = controllerName.GetHashCode();
                    result = (result * 397) ^ methodName.GetHashCode();
                    result = (result * 397) ^ verb.GetHashCode();

                    return result;
                }
            }

            public static bool operator ==(ActionRoute left, ActionRoute right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ActionRoute left, ActionRoute right)
            {
                return !left.Equals(right);
            }
        }
    }
}
