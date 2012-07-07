using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace RestFoundation.Collections.Specialized
{
    public class DynamicDictionary : DynamicObject
    {
       private readonly IDictionary m_inner;

        public int Count
        {
            get
            {
                return m_inner.Keys.Count;
            }
        }

        public DynamicDictionary() : this(new Hashtable())
        {
        }

        public DynamicDictionary(IDictionary dictionary)
        {
            if (dictionary == null) throw new ArgumentNullException("dictionary");

            m_inner = dictionary;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder == null) throw new ArgumentNullException("binder");

            result = m_inner[binder.Name];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (binder == null) throw new ArgumentNullException("binder");

            m_inner[binder.Name] = value;
            return true;
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
