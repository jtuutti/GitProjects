// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using RestFoundation.Validation;

namespace RestFoundation
{
    /// <summary>
    /// Represents the resource state for service methods.
    /// </summary>
    public class ResourceState : IEnumerable<ValidationError>
    {
        private readonly ICollection<ValidationError> m_errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceState"/> class.
        /// </summary>
        /// <param name="errors">A collection of validation errors associated with the resource.</param>
        public ResourceState(ICollection<ValidationError> errors)
        {
            if (errors == null)
            {
                throw new ArgumentNullException("errors");
            }

            m_errors = errors;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ResourceState"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return m_errors.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the resource state is valid (has no errors).
        /// </summary>
        public bool IsValid
        {
            get
            {
                return m_errors.Count == 0;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<ValidationError> GetEnumerator()
        {
            return m_errors.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_errors.GetEnumerator();
        }
    }
}
