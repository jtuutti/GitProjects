// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections;
using System.Linq;

namespace RestFoundation.Runtime
{
    internal static class NonGenericCollectionValidator
    {
        public static bool ValidateType(Type objectType)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            return objectType != typeof(IEnumerable) && objectType != typeof(ICollection) && objectType != typeof(IList) &&
                   objectType != typeof(IQueryable) && objectType != typeof(IDictionary) &&
                   !typeof(ArrayList).IsAssignableFrom(objectType) && !typeof(Hashtable).IsAssignableFrom(objectType);
        }
    }
}
