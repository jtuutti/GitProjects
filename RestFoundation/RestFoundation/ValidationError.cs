using System;

namespace RestFoundation
{
    public struct ValidationError : IEquatable<ValidationError>
    {
        private readonly string m_propertyName;
        private readonly string m_message;

        public ValidationError(string propertyName, string message) : this()
        {
            m_propertyName = propertyName;
            m_message = message;
        }

        public string PropertyName
        {
            get
            {
                return m_propertyName;
            }
        }

        public string Message
        {
            get
            {
                return m_message;
            }
        }

        public bool Equals(ValidationError other)
        {
            return Equals(other.m_propertyName, m_propertyName) && Equals(other.m_message, m_message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;

            return obj is ValidationError && Equals((ValidationError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((m_propertyName != null ? m_propertyName.GetHashCode() : 0) * 397) ^ (m_message != null ? m_message.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ValidationError left, ValidationError right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValidationError left, ValidationError right)
        {
            return !left.Equals(right);
        }
    }
}
