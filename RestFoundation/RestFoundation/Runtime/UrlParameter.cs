// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;

namespace RestFoundation.Runtime
{
    internal class UrlParameter
    {
        public static readonly UrlParameter Optional = new UrlParameter();

        private UrlParameter()
        {
        }

        public override string ToString()
        {
            return String.Empty;
        }
    }
}
