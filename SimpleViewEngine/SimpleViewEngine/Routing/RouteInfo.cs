using System;

namespace SimpleViewEngine.Routing
{
    internal class RouteInfo : IEquatable<RouteInfo>
    {
        private readonly string m_controller;
        private readonly string m_action;

        public RouteInfo(string controller, string action)
        {
            m_controller = controller;
            m_action = action;
        }

        public bool Equals(RouteInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return String.Equals(m_controller, other.m_controller, StringComparison.OrdinalIgnoreCase) &&
                   String.Equals(m_action, other.m_action, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.GetType() == GetType() && Equals((RouteInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (m_controller.GetHashCode() * 397) ^ m_action.GetHashCode();
            }
        }
    }
}
