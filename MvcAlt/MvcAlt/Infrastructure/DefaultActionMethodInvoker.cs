using System;
using System.Reflection;
using System.Web;

namespace MvcAlt.Infrastructure
{
    public class DefaultActionMethodInvoker : IActionMethodInvoker
    {
        public object Invoke(IHttpRequest request)
        {
            Delegate method = MvcConfiguration.Current.ActionMethodResolver.Resolve(request);

            object[] methodParameters;

            if (method.Method.GetParameters().Length == 1)
            {
                methodParameters = new[] { BindInputModel(request, method) };
            }
            else
            {
                methodParameters = new object[0];
            }

            return method.DynamicInvoke(methodParameters);
        }

        private static object BindInputModel(IHttpRequest request, Delegate method)
        {
            ParameterInfo parameter = method.Method.GetParameters()[0];

            string contentType = GetContentType(request);

            if (String.IsNullOrEmpty(contentType))
            {
                throw new HttpException(415, "Unrecognized content type provided");
            }

            IBinder[] binders;

            if (!MvcConfiguration.Current.BindersByType.TryGetValue(contentType, out binders))
            {
                binders = new IBinder[0];
            }

            object parameterValue = CreateParameterValue(parameter.ParameterType);

            foreach (IBinder binder in binders)
            {
                binder.Bind(request, parameter.Name, parameter.ParameterType, ref parameterValue);
            }

            return parameterValue;
        }

        private static object CreateParameterValue(Type parameterType)
        {
            if (TypeHelper.IsSimpleType(parameterType))
            {
                return Activator.CreateInstance(parameterType);
            }

            return MvcConfiguration.Current.MvcServiceLocator.GetInstance(parameterType);
        }

        private static string GetContentType(IHttpRequest request)
        {
            if (request.Verb != HttpVerb.Post && request.Verb != HttpVerb.Put && request.Verb != HttpVerb.Patch)
            {
                return "*";
            }

            string contentType = request.Headers["Content-Type"];

            if (String.IsNullOrWhiteSpace(contentType))
            {
                return null;
            }

            return contentType.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0].Trim().ToLowerInvariant();
        }
    }
}
