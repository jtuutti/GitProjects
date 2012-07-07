// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Specialized;
using System.Globalization;

namespace RestFoundation.Odata.Parser
{
    internal class ParameterParser<T> : IParameterParser<T>
    {
        private readonly IFilterExpressionFactory m_filterExpressionFactory;
        private readonly ISortExpressionFactory m_sortExpressionFactory;
        private readonly ISelectExpressionFactory<T> m_selectExpressionFactory;

        public ParameterParser()
        {
            m_filterExpressionFactory = new FilterExpressionFactory();
            m_sortExpressionFactory = new SortExpressionFactory();

            var nameResolver = new MemberNameResolver();
            m_selectExpressionFactory = new SelectExpressionFactory<T>(nameResolver, new RuntimeTypeProvider(nameResolver));
        }

        public ParameterParser(
            IFilterExpressionFactory filterExpressionFactory,
            ISortExpressionFactory sortExpressionFactory,
            ISelectExpressionFactory<T> selectExpressionFactory)
        {
            if (filterExpressionFactory == null) throw new ArgumentNullException("filterExpressionFactory");
            if (sortExpressionFactory == null) throw new ArgumentNullException("sortExpressionFactory");
            if (selectExpressionFactory == null) throw new ArgumentNullException("selectExpressionFactory");

            m_filterExpressionFactory = filterExpressionFactory;
            m_sortExpressionFactory = sortExpressionFactory;
            m_selectExpressionFactory = selectExpressionFactory;
        }

        public IModelFilter<T> Parse(NameValueCollection queryParameters)
        {
            if (queryParameters == null) throw new ArgumentNullException("queryParameters");

            var orderbyField = queryParameters[StringConstants.OrderByParameter];
            var selects = queryParameters[StringConstants.SelectParameter];
            var filter = queryParameters[StringConstants.FilterParameter];
            var skip = queryParameters[StringConstants.SkipParameter];
            var top = queryParameters[StringConstants.TopParameter];

            var filterExpression = m_filterExpressionFactory.Create<T>(filter);
            var sortDescriptions = m_sortExpressionFactory.Create<T>(orderbyField);
            var selectFunction = m_selectExpressionFactory.Create(selects);

            var modelFilter = new ModelFilter<T>(
                                            filterExpression,
                                            selectFunction,
                                            sortDescriptions,
                                            String.IsNullOrWhiteSpace(skip) ? -1 : Convert.ToInt32(skip, CultureInfo.InvariantCulture),
                                            String.IsNullOrWhiteSpace(top) ? -1 : Convert.ToInt32(top, CultureInfo.InvariantCulture));
            return modelFilter;
        }
    }
}