using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using MvcAlt.Binders;

namespace MvcAlt.Infrastructure
{
    public class ActionInvoker
    {
        public object Invoke(IHttpRequest request)
        {
            if (!request.RouteValues.ContainsKey("_controller") || !request.RouteValues.ContainsKey("_method"))
            {
                throw new HttpException(404, "Not Found");
            }

            string controllerName = request.RouteValues["_controller"].ToString();
            string methodName = request.RouteValues["_method"].ToString();

            Delegate method = Resolve(controllerName, methodName);

            if (method.Method.GetParameters().Length > 1)
            {
                throw new HttpException(500, "Action methods with multiple input parameters are not supported");
            }

            object[] methodParameters;

            if (method.Method.GetParameters().Length == 1)
            {
                ParameterInfo parameter = method.Method.GetParameters()[0];

                var binders = new IBinder[] { new RouteBinder(), new QueryStringBinder() };
                object parameterValue = Activator.CreateInstance(parameter.ParameterType);
                
                foreach (IBinder binder in binders)
                {
                    binder.Bind(parameterValue, request);
                }

                methodParameters = new[] { parameterValue };
            }
            else
            {
                methodParameters = new object[0];
            }

            return method.DynamicInvoke(methodParameters);
        }

        private static Delegate Resolve(string controllerName, string methodName)
        {
            if (String.IsNullOrEmpty(controllerName) || String.IsNullOrEmpty(methodName))
            {
                throw new HttpException(404, "Not Found");
            }

            Type controllerType = Type.GetType(String.Concat("MvcAlt.Controllers.", controllerName), false);

            if (controllerType == null)
            {
                throw new HttpException(404, "Not Found");
            }

            MethodInfo method;

            try
            {
                method = controllerType.GetMethod(methodName);
            }
            catch (AmbiguousMatchException ex)
            {
                throw new HttpException(404, "Not Found", ex);
            }

            if (method == null)
            {
                throw new HttpException(405, "Method Not Supported");
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
    }
}