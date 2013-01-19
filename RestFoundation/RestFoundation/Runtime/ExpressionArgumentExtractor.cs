// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RestFoundation.Runtime
{
    // Based on the code from: http://www.mehdi-khalili.com/extracting-input-arguments-from-expressions
    internal static class ExpressionArgumentExtractor
    {
        public static IList<ExpressionArgument> Extract(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression == null)
            {
                throw new ArgumentNullException("methodCallExpression");
            }

            var arguments = new List<ExpressionArgument>();
            ParameterInfo[] parameters = methodCallExpression.Method.GetParameters();

            for (int argumentIndex = 0; argumentIndex < methodCallExpression.Arguments.Count; argumentIndex++)
            {
                ParameterInfo parameter = parameters[argumentIndex];
                Expression argumentExpression = methodCallExpression.Arguments[argumentIndex];

                if (parameter == null || argumentExpression == null || IsResource(parameter))
                {
                    continue;
                }

                ExpressionArgument argument;

                if (TryGenerateArgument(argumentExpression, parameter, out argument))
                {
                    arguments.Add(argument);
                }
            }

            return arguments;
        }

        private static bool TryGenerateArgument(Expression argumentExpression, ParameterInfo parameter, out ExpressionArgument argument)
        {
            object value = ExtractValues(argumentExpression).FirstOrDefault();

            if (value == null)
            {
                argument = default(ExpressionArgument);
                return false;
            }

            argument = new ExpressionArgument(parameter.Name, value);
            return true;
        }

        private static IEnumerable<object> ExtractValues(Expression expression)
        {
            if (expression == null || expression is ParameterExpression)
            {
                return new object[0];
            }

            var memberExpression = expression as MemberExpression;

            if (memberExpression != null)
            {
                return ExtractValues(memberExpression);
            }

            var constantExpression = expression as ConstantExpression;

            if (constantExpression != null)
            {
                return ExtractValues(constantExpression);
            }

            var newArrayExpression = expression as NewArrayExpression;

            if (newArrayExpression != null)
            {
                return ExtractValues(newArrayExpression);
            }

            var newExpression = expression as NewExpression;

            if (newExpression != null)
            {
                return ExtractValues(newExpression);
            }

            var unaryExpression = expression as UnaryExpression;

            if (unaryExpression != null)
            {
                return ExtractValues(unaryExpression);
            }

            var methodCallExpression = expression as MethodCallExpression;

            if (methodCallExpression != null)
            {
                return ExtractValues(methodCallExpression);
            }

            return new object[0];
        }

        private static IEnumerable<object> ExtractValues(UnaryExpression unaryExpression)
        {
            return ExtractValues(unaryExpression.Operand);
        }

        private static IEnumerable<object> ExtractValues(NewExpression newExpression)
        {
            var arguments = new List<object>();

            foreach (var argumentExpression in newExpression.Arguments)
            {
                arguments.AddRange(ExtractValues(argumentExpression));
            }

            yield return newExpression.Constructor.Invoke(arguments.ToArray());
        }

        private static IEnumerable<object> ExtractValues(NewArrayExpression newArrayExpression)
        {
            Type type = newArrayExpression.Type.GetElementType();

            if (type.GetInterface(typeof(IConvertible).FullName) != null)
            {
                return ExtractConvertibleTypeArrayConstants(newArrayExpression, type);
            }

            return ExtractNonConvertibleArrayConstants(newArrayExpression, type);
        }

        private static IEnumerable<object> ExtractNonConvertibleArrayConstants(NewArrayExpression newArrayExpression, Type type)
        {
            var arrayElements = CreateList(type);

            foreach (var arrayElementExpression in newArrayExpression.Expressions)
            {
                var constantExpression = arrayElementExpression as ConstantExpression;
                object arrayElement = constantExpression != null ? constantExpression.Value : ExtractValues(arrayElementExpression).ToArray();

                var objectArrayElement = arrayElement as object[];

                if (objectArrayElement != null)
                {
                    foreach (var item in objectArrayElement)
                    {
                        arrayElements.Add(item);
                    }
                }
                else
                {
                    arrayElements.Add(arrayElement);
                }
            }

            return arrayElements.Cast<object>().ToArray();
        }

        private static IList CreateList(Type type)
        {
            ConstructorInfo defaultConstructor = typeof(List<>).MakeGenericType(type).GetConstructor(new Type[0]);

            if (defaultConstructor != null)
            {
                return (IList) defaultConstructor.Invoke(BindingFlags.CreateInstance, null, null, null);
            }

            return new object[0];
        }

        private static IEnumerable<object> ExtractConvertibleTypeArrayConstants(NewArrayExpression newArrayExpression, Type type)
        {
            IList arrayElements = CreateList(type);

            foreach (var arrayElementExpression in newArrayExpression.Expressions)
            {
                object arrayElement = ((ConstantExpression) arrayElementExpression).Value;
                arrayElements.Add(Convert.ChangeType(arrayElement, arrayElementExpression.Type, null));
            }

            return arrayElements.Cast<object>().ToList();
        }

        private static IEnumerable<object> ExtractValues(ConstantExpression constantExpression)
        {
            var constants = new List<object>();
            var valueExpression = constantExpression.Value as Expression;

            if (valueExpression != null)
            {
                constants.AddRange(ExtractValues(valueExpression));
            }
            else
            {
                if (constantExpression.Type == typeof(string) || constantExpression.Type.IsPrimitive || constantExpression.Type.IsEnum || constantExpression.Value == null)
                {
                    constants.Add(constantExpression.Value);
                }
            }

            return constants;
        }

        private static IEnumerable<object> ExtractValues(MemberExpression memberExpression)
        {
            const MemberTypes MemberTypeFlags = MemberTypes.Field | MemberTypes.Property;
            const BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            var constants = new List<object>();
            var constExpression = memberExpression.Expression as ConstantExpression;

            Type declaringType;
            object declaringObject;

            if (constExpression != null)
            {
                declaringType = constExpression.Type;
                declaringObject = constExpression.Value;
            }
            else
            {
                declaringType = memberExpression.Member.DeclaringType;
                declaringObject = ExtractValues(memberExpression.Expression).FirstOrDefault();
            }

            if (declaringType == null)
            {
                return new object[0];
            }

            var member = declaringType.GetMember(memberExpression.Member.Name, MemberTypeFlags, BindingFlags).Single();

            if (member.MemberType == MemberTypes.Field)
            {
                constants.Add(((FieldInfo) member).GetValue(declaringObject));
            }
            else
            {
                constants.Add(((PropertyInfo) member).GetGetMethod(true).Invoke(declaringObject, null));
            }

            return constants;
        }

        private static IEnumerable<object> ExtractValues(MethodCallExpression methodCallExpression)
        {
            yield return Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
        }

        // Assumes the default parameter value binder for the proxy.
        // It is a bad idea to set non-null values for resource values in the metadata so it should not be a problem.
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        private static bool IsResource(ParameterInfo parameter)
        {
            return String.Equals(ParameterValueProvider.ResourceParameterName, parameter.Name, StringComparison.OrdinalIgnoreCase) ||
                   Attribute.IsDefined(parameter, typeof(ResourceAttribute), false);
        }
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
    }
}
