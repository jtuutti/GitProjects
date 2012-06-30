using System.Text.RegularExpressions;

namespace RestFoundation.Runtime
{
    internal struct RouteParameter
    {
        private readonly string m_name;
        private readonly Regex m_contraint;

        public RouteParameter(string name, Regex contraint)
        {
            m_name = name;
            m_contraint = contraint;
        }

        public Regex Contraint
        {
            get
            {
                return m_contraint;
            }
        }

        public string Name
        {
            get
            {
                return m_name;
            }
        }
    }
}