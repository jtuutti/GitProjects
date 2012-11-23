using System;

namespace RestFoundation.Runtime
{
    internal struct ExpressionArgument
    {
        private readonly string m_name;
        private readonly object m_value;

        public ExpressionArgument(string name, object value)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            m_name = name;
            m_value = value;
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public object Value
        {
            get
            {
                return m_value;
            }
        }
    }
}
