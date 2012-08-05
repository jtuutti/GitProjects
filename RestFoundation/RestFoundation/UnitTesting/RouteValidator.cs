// <copyright>
// Dmitry Starosta, 2012
// </copyright>
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Routing;
using RestFoundation.Runtime;

namespace RestFoundation.UnitTesting
{
    internal sealed class RouteValidator<T>
    {
        private readonly HttpMethod m_httpMethod;
        private readonly string m_relativeUrl;
        private readonly Expression<Action<T>> m_serviceMethodDelegate;

        internal RouteValidator(string relativeUrl, HttpMethod httpMethod, Expression<Action<T>> serviceMethodDelegate)
        {
            if (String.IsNullOrEmpty(relativeUrl))
            {
                throw new ArgumentNullException("relativeUrl");
            }

            if (serviceMethodDelegate == null)
            {
                throw new ArgumentNullException("serviceMethodDelegate");
            }

            m_relativeUrl = relativeUrl;
            m_serviceMethodDelegate = serviceMethodDelegate;
            m_httpMethod = httpMethod;
        }

        public void Validate()
        {
            var methodDelegate = m_serviceMethodDelegate.Body as MethodCallExpression;

            if (methodDelegate == null)
            {
                throw new RouteAssertException("Invalid service method delegate provided.");
            }

            if (!typeof(T).IsInterface)
            {
                throw new RouteAssertException("Invalid service contract type provided.");
            }

            using (var httpContext = new TestHttpContext(m_relativeUrl, m_httpMethod.ToString().ToUpperInvariant()))
            {
                RouteData routeData = RouteTable.Routes.GetRouteData(httpContext);

                if (routeData == null)
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture, "URL '{0}' does not match any routes.", m_relativeUrl));
                }

                Type serviceContractType = GetServiceContractType(routeData);

                if (serviceContractType != methodDelegate.Method.DeclaringType)
                {
                    throw new RouteAssertException("Provided service contract type does not match the route.");
                }

                MethodInfo serviceMethod = GetServiceMethod(routeData, serviceContractType);

                if (serviceMethod != methodDelegate.Method)
                {
                    throw new RouteAssertException("Provided service method delegate does not match the route.");
                }

                ValidateServiceMethodArguments(methodDelegate, routeData);
            }
        }

        private static Type GetServiceContractType(RouteData routeData)
        {
            Type serviceContractType;

            try
            {
                serviceContractType = Type.GetType(routeData.GetRequiredString(RouteConstants.ServiceContractType));
            }
            catch (Exception)
            {
                serviceContractType = null;
            }

            return serviceContractType;
        }

        private static void ValidateServiceMethodArguments(MethodCallExpression methodDelegate, RouteData routeData)
        {
            ParameterInfo[] arguments = methodDelegate.Method.GetParameters();

            for (int index = 0; index < arguments.Length; index++)
            {
                ParameterInfo argument = arguments[index];

                if (!routeData.Values.ContainsKey(argument.Name))
                {
                    continue;
                }

                Expression argumentExpression = methodDelegate.Arguments[index];

                if (argumentExpression == null)
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture, "There was a problem validating service method argument '{0}'.", argument.Name));
                }

                object argumentValue = GetArgumentValue(argument, argumentExpression);
                object routeArgumentValue = routeData.Values[argument.Name];
                object convertedArgumentValue;

                if (!SafeConvert.TryChangeType(routeArgumentValue, argumentExpression.Type, out convertedArgumentValue) || !Equals(argumentValue, convertedArgumentValue))
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture,
                                                                 "Service method delegate value of the argument '{0}' does not match the corresponding route value: {1} != {2}",
                                                                 argument.Name,
                                                                 argumentValue ?? "(null)",
                                                                 routeArgumentValue ?? "(null)"));
                }

                var constraintAttribute = Attribute.GetCustomAttribute(argument, typeof(ParameterConstraintAttribute), false) as ParameterConstraintAttribute;

                if (constraintAttribute != null && !constraintAttribute.PatternRegex.IsMatch(Convert.ToString(routeArgumentValue, CultureInfo.InvariantCulture)))
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture,
                                                                 "Route parameter '{0}' with value '{1}' does not match the constraint pattern '{2}'.",
                                                                  argument.Name,
                                                                  routeArgumentValue,
                                                                  constraintAttribute.Pattern));
                }
            }
        }

        private static object GetArgumentValue(ParameterInfo argument, Expression argumentExpression)
        {
            object argumentValue;

            if (argumentExpression is ConstantExpression)
            {
                argumentValue = ((ConstantExpression) argumentExpression).Value;
            }
            else if (argumentExpression is UnaryExpression)
            {
                Expression unaryExpression = ((UnaryExpression) argumentExpression).Operand;

                if (unaryExpression is ConstantExpression)
                {
                    argumentValue = ((ConstantExpression) unaryExpression).Value;
                }
                else if (unaryExpression is MethodCallExpression)
                {
                    argumentValue = Expression.Lambda(unaryExpression).Compile().DynamicInvoke();
                }
                else
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture,
                                                                 "Method argument '{0}' has a value that is to complex to process. Pass in a constant/variable into the delegate instead.",
                                                                 argument.Name));
                }
            }
            else if (argumentExpression is MethodCallExpression)
            {
                argumentValue = Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
            }
            else
            {
                throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture,
                                                             "Method argument '{0}' has a value that is to complex to process. Pass in a constant/variable into the delegate instead.",
                                                             argument.Name));
            }

            return argumentValue;
        }

        private MethodInfo GetServiceMethod(RouteData routeData, Type serviceContractType)
        {
            MethodInfo serviceMethod;

            try
            {
                serviceMethod = ServiceMethodRegistry.GetMethod(new ServiceMetadata(serviceContractType,
                                                                                    routeData.GetRequiredString(RouteConstants.ServiceUrl)),
                                                                                    routeData.GetRequiredString(RouteConstants.UrlTemplate),
                                                                                    m_httpMethod);
            }
            catch (Exception)
            {
                serviceMethod = null;
            }

            return serviceMethod;
        }
    }
}
