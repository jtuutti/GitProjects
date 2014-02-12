// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RestFoundation.ServiceProxy
{
    internal sealed class XmlDocMetadata
    {
        public XmlDocMetadata()
        {
            Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public MethodInfo Method { get; set; }

        public string Summary { get; set; }

        public string Remarks { get; set; }

        public string Returns { get; set; }

        public IDictionary<string, string> Parameters { get; private set; }
    }
}
