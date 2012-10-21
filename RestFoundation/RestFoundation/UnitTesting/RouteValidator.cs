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
                throw new RouteAssertException(RestResources.InvalidServiceMethodExpression);
            }

            Type validatedContractType = typeof(T);

            if (!validatedContractType.IsInterface && !validatedContractType.IsClass)
            {
                throw new RouteAssertException(RestResources.InvalidServiceContract);
            }

            if (validatedContractType.IsClass && (validatedContractType.IsAbstract || Attribute.GetCustomAttribute(validatedContractType, typeof(ServiceContractAttribute), true) == null))
            {
                throw new RouteAssertException(RestResources.InvalidServiceImplementation);
            }

            using (var httpContext = new TestHttpContext(m_relativeUrl, m_httpMethod.ToString().ToUpperInvariant()))
            {
                RouteData routeData = RouteTable.Routes.GetRouteData(httpContext);

                if (routeData == null)
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture, RestResources.MismatchedUrl, m_relativeUrl));
                }

                Type contractType = GetServiceContractType(routeData);

                if (contractType != methodDelegate.Method.DeclaringType)
                {
                    throw new RouteAssertException(RestResources.MismatchedServiceMethodRoute);
                }

                MethodInfo serviceMethod = GetServiceMethod(routeData, contractType);

                if (serviceMethod != methodDelegate.Method)
                {
                    throw new RouteAssertException(RestResources.InvalidServiceMethodExpression);
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
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture, RestResources.InvalidServiceMethodArgument, argument.Name));
                }

                object argumentValue = GetArgumentValue(argument, argumentExpression);
                object routeArgumentValue = routeData.Values[argument.Name];
                object convertedArgumentValue;

                if (!SafeConvert.TryChangeType(routeArgumentValue, argumentExpression.Type, out convertedArgumentValue) || !Equals(argumentValue, convertedArgumentValue))
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture,
                                                                 RestResources.MismatchedServiceMethodExpression,
                                                                 argument.Name,
                                                                 argumentValue ?? "(null)",
                                                                 routeArgumentValue ?? "(null)"));
                }

                var constraintAttribute = Attribute.GetCustomAttribute(argument, typeof(ParameterConstraintAttribute), false) as ParameterConstraintAttribute;

                if (constraintAttribute != null && !constraintAttribute.PatternRegex.IsMatch(Convert.ToString(routeArgumentValue, CultureInfo.InvariantCulture)))
                {
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture,
                                                                 RestResources.ConstraintMismatchedRouteParameter,
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
                    throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture, RestResources.OvercomplicatedMethodArgument, argument.Name));
                }
            }
            else if (argumentExpression is MethodCallExpression)
            {
                argumentValue = Expression.Lambda(argumentExpression).Compile().DynamicInvoke();
            }
            else
            {
                throw new RouteAssertException(String.Format(CultureInfo.InvariantCulture, RestResources.OvercomplicatedMethodArgument, argument.Name));
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
