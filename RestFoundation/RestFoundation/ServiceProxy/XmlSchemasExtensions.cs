﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy
{
    public static class XmlSchemasExtensions
    {
        private static readonly Regex arrayPattern = new Regex("ArrayOf([A-Za-z0-9_]+)", RegexOptions.Compiled);

        public static IList<string> Serialize(this XmlSchemas schemas)
        {
            if (schemas == null)
            {
                return null;
            }

            return schemas.Select(ConvertSchemaToString)
                          .Where(schemaString => !String.IsNullOrEmpty(schemaString))
                          .Distinct()
                          .ToList();
        }

        private static string ConvertSchemaToString(XmlSchema schema)
        {
            using (var stream = new MemoryStream())
            {
                schema.Write(stream);

                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(stream, Encoding.UTF8);
                string schemaXml = reader.ReadToEnd();

                if (!String.IsNullOrWhiteSpace(schemaXml))
                {
                    return FormatXml(schemaXml);
                }
            }

            return null;
        }

        private static string FormatXml(string schemaXml)
        {
            if (String.IsNullOrWhiteSpace(schemaXml))
            {
                return schemaXml;
            }

            var pluralization = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            schemaXml = arrayPattern.Replace(schemaXml, match => pluralization.Pluralize(match.Result("$1")));

            XDocument document;

            try
            {
                document = XDocument.Parse(schemaXml);
                document.Declaration = new XDeclaration("1.0", "utf-8", null);
            }
            catch (Exception)
            {
                return schemaXml;
            }

            return String.Format("{0}{1}{2}", document.Declaration, Environment.NewLine, document.ToString(SaveOptions.OmitDuplicateNamespaces));
        }
    }
}