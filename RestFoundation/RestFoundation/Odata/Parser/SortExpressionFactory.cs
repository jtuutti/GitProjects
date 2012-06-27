// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;

namespace RestFoundation.Odata.Parser
{
	/// <summary>
	/// Defines the SortExpressionFactory´.
	/// </summary>
	public class SortExpressionFactory : ISortExpressionFactory
	{
		private static readonly CultureInfo defaultCulture = CultureInfo.GetCultureInfo("en-US");

		/// <summary>
		/// Creates an enumeration of sort descriptions from its string representation.
		/// </summary>
		/// <param name="filter">The string representation of the sort descriptions.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item to sort.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> if the passed sort descriptions are valid, otherwise null.</returns>
		public IEnumerable<SortDescription<T>> Create<T>(string filter)
		{
			return Create<T>(filter, defaultCulture);
		}

		/// <summary>
		/// Creates an enumeration of sort descriptions from its string representation.
		/// </summary>
		/// <param name="filter">The string representation of the sort descriptions.</param>
		/// <param name="formatProvider">The <see cref="IFormatProvider"/> to use when reading the sort descriptions.</param>
		/// <typeparam name="T">The <see cref="Type"/> of item to sort.</typeparam>
		/// <returns>An <see cref="IEnumerable{T}"/> if the passed sort descriptions are valid, otherwise null.</returns>
		public IEnumerable<SortDescription<T>> Create<T>(string filter, IFormatProvider formatProvider)
		{
			if (string.IsNullOrWhiteSpace(filter))
			{
				return new SortDescription<T>[0];
			}

			var parameterExpression = Expression.Parameter(typeof(T), "x");

			var sortTokens = filter.Split(',');
			return from sortToken in sortTokens
				   select sortToken.Split(' ')
					   into sort
					   let property = GetPropertyExpression<T>(sort.First(), parameterExpression)
					   let direction = sort.ElementAtOrDefault(1) == "desc" ? SortDirection.Descending : SortDirection.Ascending
					   where property != null
					   select new SortDescription<T>(property.Compile(), direction);
		}

		private Expression<Func<T, object>> GetPropertyExpression<T>(string propertyToken, ParameterExpression parameter)
		{
		    if (propertyToken == null) throw new ArgumentNullException("propertyToken");

		    var parentType = typeof(T);
			Expression propertyExpression = null;
			var propertyChain = propertyToken.Split('/');
			foreach (var propertyName in propertyChain)
			{
				var property = parentType.GetProperty(propertyName);
				if (property != null)
				{
					parentType = property.PropertyType;
					propertyExpression = propertyExpression == null
											? Expression.Convert(Expression.Property(parameter, property), typeof(object))
											: Expression.Convert(Expression.Property(propertyExpression, property), typeof(object));
				}
			}

			return propertyExpression == null ? null : Expression.Lambda<Func<T, object>>(propertyExpression, parameter);
		}
	}
}