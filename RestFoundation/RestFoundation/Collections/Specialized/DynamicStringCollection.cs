using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using RestFoundation.Collections.Concrete;

namespace RestFoundation.Collections.Specialized
{
    /// <summary>
    /// Represents a dynamic string collection.
    /// </summary>
    public class DynamicStringCollection : DynamicObject
    {
        private readonly IStringValueCollection m_inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicStringCollection"/> class.
        /// </summary>
        public DynamicStringCollection() : this(new StringValueCollection(new NameValueCollection()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicStringCollection"/> class.
        /// </summary>
        /// <param name="collection">The static string value collection to convert to dynamic.</param>
        public DynamicStringCollection(IStringValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            m_inner = collection;
        }

        /// <summary>
        /// Gets a value containing the count of items in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return m_inner.Keys.Count;
            }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="DynamicStringCollection"/>
        /// class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        /// <param name="binder">Provides information about the object that called the dynamic operation.</param>
        /// <param name="result">The result of the get operation.</param>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            ICollection<string> values = m_inner.GetValues(binder.Name);

            if (binder.Name.IndexOf('_') > 0 && (values == null || values.Count == 0))
            {
                values = m_inner.GetValues(binder.Name.Replace('_', '-'));
            }

            if (values == null || values.Count == 0)
            {
                result = String.Empty;
            }
            else if (values.Count == 1)
            {
                result = values.First();
            }
            else
            {
                result = values;
            }

            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="DynamicStringCollection"/>
        /// class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation.The binder.Name property provides the name
        /// of the member to which the value is being assigned.
        /// </param>
        /// <param name="value">The value to set to the member</param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="DynamicStringCollection"/>
        /// class can override this method to specify dynamic behavior for operations such as calling a method.
        /// </summary>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        /// <param name="binder">Provides information about the dynamic operation.</param>
        /// <param name="args">The arguments that are passed to the object member during the invoke operation.</param>
        /// <param name="result">The result of the member invocation.</param>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Provides the implementation for operations that delete an object member.
        /// </summary>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        /// <param name="binder">Provides information about the deletion.</param>
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the enumeration of all dynamic member names. 
        /// </summary>
        /// <returns>
        /// A sequence that contains dynamic member names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (string name in m_inner.Keys)
            {
                yield return name;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return m_inner.ToString();
        }
    }
}
