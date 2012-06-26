using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using RestFoundation.Collections.Concrete;

namespace RestFoundation.Collections.Specialized
{
    public class DynamicStringCollection : DynamicObject
    {
       private readonly IStringValueCollection m_inner;

        public int Count
        {
            get
            {
                return m_inner.Keys.Count;
            }
        }

        public DynamicStringCollection() : this(new StringValueCollection(new NameValueCollection()))
        {
        }

        public DynamicStringCollection(IStringValueCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            m_inner = collection;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
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

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new NotSupportedException();
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            throw new NotSupportedException();
        }

        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (string name in m_inner.Keys)
            {
                yield return name;
            }
        }

        public override string ToString()
        {
            return m_inner.ToString();
        }
    }
}
