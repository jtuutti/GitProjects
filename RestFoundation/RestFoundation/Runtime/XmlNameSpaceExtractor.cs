// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Runtime
{
    internal static class XmlNameSpaceExtractor
    {
        public static string Get()
        {
            if (Rest.Configuration.Options.XmlSettings == null)
            {
                return String.Empty;
            }

            if (String.IsNullOrWhiteSpace(Rest.Configuration.Options.XmlSettings.NameSpace))
            {
                return String.Empty;
            }

            return Rest.Configuration.Options.XmlSettings.NameSpace.Trim();
        }
    }
}
