using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace RestFoundation.Collections.Specialized
{
    /// <summary>
    /// Represents a dynamic dictionary collection.
    /// </summary>
    public class DynamicDictionary : DynamicObject
    {
        private readonly IDictionary m_inner;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary"/> class.
        /// </summary>
        public DynamicDictionary() : this(new Hashtable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">The static dictionary to convert to dynamic.</param>
        public DynamicDictionary(IDictionary dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }

            m_inner = dictionary;
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
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="DynamicDictionary"/> class
        /// can override this method to specify dynamic behavior for operations such as getting a value for a property.
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

            result = m_inner[binder.Name];
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="DynamicDictionary"/> class
        /// can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <returns>true if the operation is successful; otherwise, false.</returns>
        /// <param name="binder">
        /// Provides information about the object that called the dynamic operation.The binder.Name property provides the name
        /// of the member to which the value is being assigned.
        /// </param>
        /// <param name="value">The value to set to the member</param>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            m_inner[binder.Name] = value;
            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that invoke a member. Classes derived from the <see cref="DynamicDictionary"/> class
        /// can override this method to specify dynamic behavior for operations such as calling a method.
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
            var output = new StringBuilder();

            foreach (DictionaryEntry item in m_inner)
            {
                if (output.Length > 0)
                {
                    output.Append(", ");
                }

                output.Append(item.Key);
            }

            return output.ToString();
        }
    }
}
