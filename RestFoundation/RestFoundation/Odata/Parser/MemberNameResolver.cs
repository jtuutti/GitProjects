// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RestFoundation.Odata.Parser
{
    internal class MemberNameResolver : IMemberNameResolver
    {
        private static readonly ConcurrentDictionary<MemberInfo, string> knownMemberNames = new ConcurrentDictionary<MemberInfo, string>();

        public string ResolveName(MemberInfo member)
        {
            return knownMemberNames.GetOrAdd(member, ResolveNameInternal);
        }

        private static string ResolveNameInternal(MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException("member");

            var jsonProperty = member.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
                .OfType<JsonPropertyAttribute>()
                .FirstOrDefault();

            if (jsonProperty != null && jsonProperty.PropertyName != null)
            {
                return jsonProperty.PropertyName;
            }

            var xmlElement = member.GetCustomAttributes(typeof(XmlElementAttribute), true)
                .OfType<XmlElementAttribute>()
                .FirstOrDefault();

            if (xmlElement != null && xmlElement.ElementName != null)
            {
                return xmlElement.ElementName;
            }

            var xmlAttribute = member.GetCustomAttributes(typeof(XmlAttributeAttribute), true)
                .OfType<XmlAttributeAttribute>()
                .FirstOrDefault();

            if (xmlAttribute != null && xmlAttribute.AttributeName != null)
            {
                return xmlAttribute.AttributeName;
            }

            return member.Name;
        }
    }
}