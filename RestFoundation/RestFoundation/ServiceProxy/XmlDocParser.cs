// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using RestFoundation.Resources;
using RestFoundation.Runtime;

namespace RestFoundation.ServiceProxy
{
    internal sealed class XmlDocParser
    {
        private readonly Type m_contractType;
        private readonly string m_filePath;

        public XmlDocParser(Type contractType, string filePath)
        {
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            if (!ServiceContractTypeRegistry.IsServiceContract(contractType))
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, Global.InvalidServiceContractType, contractType.FullName),
                                            "contractType");
            }

            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException("filePath");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(Global.InvalidFilePathOrUrl, filePath);
            }

            m_filePath = filePath;
            m_contractType = contractType;
        }

        public IReadOnlyList<XmlDocMetadata> GetMetadata()
        {
            var xmlDoc = XDocument.Load(m_filePath);

            if (xmlDoc.Root == null)
            {
                return new XmlDocMetadata[0];
            }

            var metadataList = new List<XmlDocMetadata>();

            foreach (MethodInfo method in m_contractType.GetMethods())
            {
                XmlDocMetadata metadata = GetMethodMetadata(xmlDoc, method);

                if (metadata != null)
                {
                    metadataList.Add(metadata);
                }
            }

            return metadataList.AsReadOnly();
        }
        
        private static string GenerateParameterName(ParameterInfo parameter)
        {
            if (!parameter.ParameterType.IsGenericType)
            {
                return parameter.ParameterType.FullName;
            }

            var parameterNameBuilder = new StringBuilder();

            string genericTypeName = parameter.ParameterType.GetGenericTypeDefinition().FullName;
            parameterNameBuilder.Append(genericTypeName.Substring(0, genericTypeName.IndexOf('`'))).Append('{');

            Type[] genericParameters = parameter.ParameterType.GetGenericArguments();

            for (int i = 0; i < genericParameters.Length; i++)
            {
                if (i > 0)
                {
                    parameterNameBuilder.Append(',');
                }

                parameterNameBuilder.Append(genericParameters[0].FullName);
            }

            parameterNameBuilder.Append('}');

            return parameterNameBuilder.ToString();
        }

        private string GetMethodSignature(MethodInfo method, ParameterInfo[] parameters)
        {
            var methodSignatureBuilder = new StringBuilder();
            methodSignatureBuilder.AppendFormat(CultureInfo.InvariantCulture, "M:{0}.{1}", m_contractType.FullName, method.Name);

            if (parameters.Length == 0)
            {
                return methodSignatureBuilder.ToString();
            }

            methodSignatureBuilder.Append('(');

            for (int i = 0; i < parameters.Length; i++)
            {
                methodSignatureBuilder.Append(GenerateParameterName(parameters[i]));

                if (i < parameters.Length - 1)
                {
                    methodSignatureBuilder.Append(',');
                }
            }

            methodSignatureBuilder.Append(')');

            return methodSignatureBuilder.ToString();
        }

        private XmlDocMetadata GetMethodMetadata(XDocument xmlDoc, MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            string methodSignature = GetMethodSignature(method, parameters);

            if (xmlDoc.Root == null)
            {
                return null;
            }

            XElement methodDoc = xmlDoc.Root.Elements("members")
                                            .Elements("member")
                                            .FirstOrDefault(x => x.Attribute("name") != null && x.Attribute("name").Value == methodSignature);

            if (methodDoc == null)
            {
                return null;
            }

            XElement summaryDoc = methodDoc.Element("summary");
            XElement remarksDoc = methodDoc.Element("remarks");
            XElement returnsDoc = methodDoc.Element("returns");

            var metadata = new XmlDocMetadata
            {
                Method = method,
                Summary = summaryDoc != null ? summaryDoc.Value.Trim() : String.Empty,
                Remarks = remarksDoc != null ? remarksDoc.Value.Trim() : String.Empty,
                Returns = returnsDoc != null ? returnsDoc.Value.Trim() : String.Empty,
            };

            if (parameters.Length == 0)
            {
                return metadata;
            }

            foreach (ParameterInfo parameter in parameters)
            {
                XElement parameterDoc = methodDoc.Elements("param")
                                                 .FirstOrDefault(x => x.Attribute("name") != null && x.Attribute("name").Value == parameter.Name);

                if (parameterDoc != null)
                {
                    metadata.Parameters[parameter.Name] = parameterDoc.Value.Trim();
                }
            }

            return metadata;
        }
    }
}
