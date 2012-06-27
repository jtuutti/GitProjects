// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace RestFoundation.Odata.Parser
{
    /// <summary>
    /// Defines the SelectExpressionFactory.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of object to project.</typeparam>
    public class SelectExpressionFactory<T> : ISelectExpressionFactory<T>
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private readonly IMemberNameResolver m_nameResolver;
        private readonly IRuntimeTypeProvider m_runtimeTypeProvider;
        private readonly IDictionary<string, Expression<Func<T, object>>> m_knownSelections;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectExpressionFactory{T}"/> class.
        /// </summary>
        public SelectExpressionFactory(IMemberNameResolver nameResolver, IRuntimeTypeProvider runtimeTypeProvider)
        {
            if (nameResolver == null) throw new ArgumentNullException("nameResolver");
            if (runtimeTypeProvider == null) throw new ArgumentNullException("runtimeTypeProvider");

            m_nameResolver = nameResolver;
            m_runtimeTypeProvider = runtimeTypeProvider;
            m_knownSelections = new Dictionary<string, Expression<Func<T, object>>>
                                {
                                    { string.Empty, null }
                                };
        }

        /// <summary>
        /// Creates a select expression.
        /// </summary>
        /// <param name="selection">The properties to select.</param>
        /// <returns>An instance of a <see cref="Func{T1,TResult}"/>.</returns>
        public Expression<Func<T, object>> Create(string selection)
        {
            var fieldNames = (selection ?? string.Empty).Split(',')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .OrderBy(x => x);

            var key = string.Join(",", fieldNames);

            if (m_knownSelections.ContainsKey(key))
            {
                var knownSelection = m_knownSelections[key];

                return knownSelection;
            }

            var elementType = typeof(T);
            var elementMembers = elementType.GetProperties(Flags)
                .Cast<MemberInfo>()
                .Concat(elementType.GetFields(Flags))
                .ToArray();

            var sourceMembers = fieldNames.ToDictionary(name => name, s => elementMembers.First(m => m_nameResolver.ResolveName(m) == s));
            var dynamicType = m_runtimeTypeProvider.Get(elementType, sourceMembers.Values);

            var sourceItem = Expression.Parameter(elementType, "t");
            var bindings = dynamicType
                .GetProperties()
                .Select(p =>
                            {
                                var member = sourceMembers[p.Name];
                                var expression = member.MemberType == MemberTypes.Property
                                                    ? Expression.Property(sourceItem, (PropertyInfo)member)
                                                    : Expression.Field(sourceItem, (FieldInfo)member);
                                return Expression.Bind(p, expression);
                            });

            var constructorInfo = dynamicType.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null) throw new InvalidOperationException("Created type does not have a default constructor");

            var selector = Expression.Lambda<Func<T, object>>(
                                                              Expression.MemberInit(Expression.New(constructorInfo), bindings),
                                                              sourceItem);

            if (Monitor.TryEnter(m_knownSelections, 1000))
            {
                try
                {
                    m_knownSelections.Add(key, selector);
                }
                finally
                {
                    Monitor.Exit(m_knownSelections);
                }
            }

            return selector;
        }
    }
}