// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using RestFoundation.Runtime;
using RestFoundation.TypeBinders;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Represents a type binder builder.
    /// </summary>
    public sealed class TypeBinderBuilder
    {
        internal TypeBinderBuilder()
        {
        }

        /// <summary>
        /// Gets an associated type binder by the object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>The associated type binder or null.</returns>
        public ITypeBinder Get(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            return TypeBinderRegistry.GetBinder(objectType);
        }

        /// <summary>
        /// Gets a sequence of all the type binders.
        /// </summary>
        /// <returns>The type binders.</returns>
        public IEnumerable<ITypeBinder> GetAll()
        {
            return TypeBinderRegistry.GetBinders().Distinct();
        }

        /// <summary>
        /// Sets a type binder for the provided object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <param name="binder">The type binder.</param>
        public void Set(Type objectType, ITypeBinder binder)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (binder == null)
            {
                throw new ArgumentNullException("binder");
            }

            TypeBinderRegistry.SetBinder(objectType, binder);
        }

        /// <summary>
        /// Removes an associated type binder for the provided object type.
        /// </summary>
        /// <param name="objectType">The object type.</param>
        /// <returns>
        /// true if a type binder was removed; false if no type binder had been associated
        /// for the object type.
        /// </returns>
        public bool Remove(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            return TypeBinderRegistry.RemoveBinder(objectType);
        }

        /// <summary>
        /// Removes the provided type binders.
        /// </summary>
        /// <param name="binder">The type binder.</param>
        /// <returns>true if the type binder was removed; false otherwise.</returns>
        public bool Remove(ITypeBinder binder)
        {
            return TypeBinderRegistry.RemoveBinder(binder);
        }

        /// <summary>
        /// Clears all associated type binders.
        /// </summary>
        public void Clear()
        {
            TypeBinderRegistry.ClearBinders();
        }
    }
}
