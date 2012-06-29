using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace RestFoundation.Runtime
{
    public static class XmlSchemaGenerator
    {
        public static XmlSchemas Generate<T>()
            where T : class
        {
            Type objectType = typeof(T);

            var schemas = new XmlSchemas();

            try
            {
                GetSchemaByReflection(objectType, schemas);
            }
            catch (Exception)
            {
                object model = TryInstantiateModel(objectType);

                if (model != null)
                {
                    GetSchemaByInference(model, schemas);
                }
            }

            return schemas;
        }

        private static object TryInstantiateModel(Type modelType)
        {
            try
            {
                object model;
                    
                if (modelType.IsArray)
                {
                    model = Activator.CreateInstance(modelType, 1);
                }
                else if (modelType == typeof(string))
                {
                    model = String.Empty;
                }
                else
                {
                    model = Activator.CreateInstance(modelType, true);
                }

                MethodInfo setMethod = modelType.GetMethod("SetValue", new[] { typeof(object), typeof(int) });

                if (setMethod != null)
                {
                    setMethod.Invoke(model, new[] { TryInstantiateModel(modelType.GetElementType()), 0 });
                }

                if (!modelType.IsArray && !modelType.IsPrimitive)
                {
                    foreach (PropertyInfo property in model.GetType().GetProperties())
                    {
                        if (!property.CanWrite || property.GetIndexParameters().Length > 0) continue;

                        object propertyValue = TryInstantiateModel(property.PropertyType);
                        if (propertyValue == null) continue;

                        property.SetValue(model, propertyValue, null);
                    }
                }

                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void GetSchemaByInference(object model, XmlSchemas schemas)
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    var serializer = new XmlSerializer(model.GetType());
                    serializer.Serialize(stream, model);
                }
                catch (Exception)
                {
                    return;
                }

                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                var reader = XmlReader.Create(stream);
                var schemaInferrer = new XmlSchemaInference();

                XmlSchemaSet schemaSet = schemaInferrer.InferSchema(reader);

                foreach (XmlSchema schema in schemaSet.Schemas())
                {
                    schemas.Add(schema);
                }
            }
        }

        private static void GetSchemaByReflection(Type modelType, XmlSchemas schemas)
        {
            var importer = new SoapReflectionImporter();
            var exporter = new XmlSchemaExporter(schemas);

            XmlTypeMapping modelMap = importer.ImportTypeMapping(modelType);
            exporter.ExportTypeMapping(modelMap);

            schemas.Compile(null, false);
        }
    }
}
