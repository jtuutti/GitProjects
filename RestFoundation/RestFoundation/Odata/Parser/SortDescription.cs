// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Web.UI.WebControls;

namespace RestFoundation.Odata.Parser
{
	/// <summary>
	/// Defines a sort description.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to sort.</typeparam>
	public class SortDescription<T>
	{
		private readonly Func<T, object> m_keySelector;
		private readonly SortDirection m_direction;

		/// <summary>
		/// Initializes a new instance of the <see cref="SortDescription{T}"/> class.
		/// </summary>
		/// <param name="keySelector">The function to select the sort key.</param>
		/// <param name="direction">The sort direction.</param>
		public SortDescription(Func<T, object> keySelector, SortDirection direction)
		{
		    if (keySelector == null) throw new ArgumentNullException("keySelector");

		    m_keySelector = keySelector;
			m_direction = direction;
		}

		/// <summary>
		/// Gets the sort direction.
		/// </summary>
		public SortDirection Direction
		{
			get { return m_direction; }
		}

		/// <summary>
		/// Gets the key to sort by.
		/// </summary>
		public Func<T, object> KeySelector
		{
			get { return m_keySelector; }
		}
	}
}