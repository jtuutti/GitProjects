// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using RestFoundation.Odata.Parser;

namespace RestFoundation.Odata
{
    [ExcludeFromCodeCoverage]
    internal class ModelFilter<T> : IModelFilter<T>
    {
        private readonly int m_skip;
        private readonly int m_top;
        private readonly Expression<Func<T, bool>> m_filterExpression;
        private readonly Expression<Func<T, object>> m_selectExpression;
        private readonly IEnumerable<SortDescription<T>> m_sortDescriptions;

        public ModelFilter(Expression<Func<T, bool>> filterExpression, Expression<Func<T, object>> selectExpression, IEnumerable<SortDescription<T>> sortDescriptions, int skip, int top)
        {
            m_skip = skip;
            m_top = top;
            m_filterExpression = filterExpression;
            m_selectExpression = selectExpression;
            m_sortDescriptions = sortDescriptions;
        }

        public IEnumerable<object> Filter(IEnumerable<T> model)
        {
            var result = m_filterExpression != null ? model.AsQueryable().Where(m_filterExpression) : model;

            if (m_sortDescriptions != null && m_sortDescriptions.Any())
            {
                var isFirst = true;
                foreach (var sortDescription in m_sortDescriptions)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        result = sortDescription.Direction == SortDirection.Ascending
                            ? result.OrderBy(sortDescription.KeySelector)
                            : result.OrderByDescending(sortDescription.KeySelector);
                    }
                    else
                    {
                        var orderedEnumerable = (IOrderedEnumerable<T>) result;

                        result = sortDescription.Direction == SortDirection.Ascending
                                    ? orderedEnumerable.ThenBy(sortDescription.KeySelector)
                                    : orderedEnumerable.ThenByDescending(sortDescription.KeySelector);
                    }
                }
            }

            if (m_skip > 0)
            {
                result = result.Skip(m_skip);
            }

            if (m_top > -1)
            {
                result = result.Take(m_top);
            }

            return m_selectExpression == null
                    ? result.OfType<object>().ToArray()
                    : result.ToArray().AsQueryable().Select(m_selectExpression).ToArray();
        }
    }
}