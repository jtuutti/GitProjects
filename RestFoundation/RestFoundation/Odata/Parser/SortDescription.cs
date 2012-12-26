// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI.WebControls;

namespace RestFoundation.Odata.Parser
{
    [ExcludeFromCodeCoverage]
    internal class SortDescription<T>
    {
        private readonly Func<T, object> m_keySelector;
        private readonly SortDirection m_direction;

        public SortDescription(Func<T, object> keySelector, SortDirection direction)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            m_keySelector = keySelector;
            m_direction = direction;
        }

        public SortDirection Direction
        {
            get { return m_direction; }
        }

        public Func<T, object> KeySelector
        {
            get { return m_keySelector; }
        }
    }
}