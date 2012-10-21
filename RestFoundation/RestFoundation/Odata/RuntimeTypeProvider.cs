// (c) Copyright Reimers.dk.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://www.opensource.org/licenses/MS-PL] for details.
// All other rights reserved.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using RestFoundation.Odata.Parser;

namespace RestFoundation.Odata
{
    internal class RuntimeTypeProvider : IRuntimeTypeProvider
    {
        private const MethodAttributes GetSetAttr = MethodAttributes.Final | MethodAttributes.Public;

        private static readonly AssemblyName assemblyName = new AssemblyName { Name = "Linq2RestTypes" };
        private static readonly ModuleBuilder moduleBuilder;
        private static readonly Dictionary<string, Type> builtTypes = new Dictionary<string, Type>();
        private static readonly ConcurrentDictionary<Type, CustomAttributeBuilder[]> typeAttributeBuilders = new ConcurrentDictionary<Type, CustomAttributeBuilder[]>();
        private static readonly ConcurrentDictionary<MemberInfo, CustomAttributeBuilder[]> propertyAttributeBuilders = new ConcurrentDictionary<MemberInfo, CustomAttributeBuilder[]>();

        private readonly IMemberNameResolver m_nameResolver;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "This is Microsoft code")]
        static RuntimeTypeProvider()
        {
            moduleBuilder = Thread
                .GetDomain()
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(assemblyName.Name);
        }

        public RuntimeTypeProvider(IMemberNameResolver nameResolver)
        {
            if (nameResolver == null)
            {
                throw new ArgumentNullException("nameResolver");
            }

            m_nameResolver = nameResolver;
        }

        public Type Get(Type sourceType, IEnumerable<MemberInfo> properties)
        {
            properties = properties.ToArray();

            if (!properties.Any())
            {
                throw new ArgumentOutOfRangeException("properties", RestResources.MissingPropertyDefinition);
            }

            var dictionary = properties.ToDictionary(f => m_nameResolver.ResolveName(f), f => f);
            string className;

            Monitor.Enter(builtTypes);

            try
            {
                className = GetTypeKey(sourceType, dictionary);
                
                if (builtTypes.ContainsKey(className))
                {
                    return builtTypes[className];
                }

                var typeBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                SetAttributes(typeBuilder, sourceType);

                foreach (var field in dictionary)
                {
                    CreateProperty(typeBuilder, field);
                }

                builtTypes[className] = typeBuilder.CreateType();
            }
            finally
            {
                Monitor.Exit(builtTypes);
            }

            return builtTypes[className];
        }

        private static void CreateProperty(TypeBuilder typeBuilder, KeyValuePair<string, MemberInfo> field)
        {
            if (typeBuilder == null)
            {
                throw new ArgumentNullException("typeBuilder");
            }

            var propertyType = field.Value.MemberType == MemberTypes.Property
                                ? ((PropertyInfo)field.Value).PropertyType
                                : ((FieldInfo)field.Value).FieldType;
            var fieldBuilder = typeBuilder.DefineField("_" + field.Key, propertyType, FieldAttributes.Private);

            var propertyBuilder = typeBuilder.DefineProperty(field.Key, PropertyAttributes.None, propertyType, null);

            SetAttributes(propertyBuilder, field.Value);

            var getAccessor = typeBuilder.DefineMethod(
                                                       "get_" + field.Key,
                                                       GetSetAttr,
                                                       propertyType,
                                                       Type.EmptyTypes);

            var getIl = getAccessor.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setAccessor = typeBuilder.DefineMethod(
                                                       "set_" + field.Key,
                                                       GetSetAttr,
                                                       null,
                                                       new[] { propertyType });

            var setIl = setAccessor.GetILGenerator();
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getAccessor);
            propertyBuilder.SetSetMethod(setAccessor);
        }

        private static void SetAttributes(TypeBuilder typeBuilder, Type type)
        {
            if (typeBuilder == null)
            {
                throw new ArgumentNullException("typeBuilder");
            }

            var attributeBuilders = typeAttributeBuilders
                .GetOrAdd(
                          type,
                          t =>
                            {
                                var customAttributes = t.GetCustomAttributesData();
                                return CreateCustomAttributeBuilders(customAttributes).ToArray();
                            });

            foreach (var attributeBuilder in attributeBuilders)
            {
                typeBuilder.SetCustomAttribute(attributeBuilder);
            }
        }

        private static void SetAttributes(PropertyBuilder propertyBuilder, MemberInfo memberInfo)
        {
            if (propertyBuilder == null)
            {
                throw new ArgumentNullException("propertyBuilder");
            }

            if (memberInfo == null)
            {
                throw new ArgumentNullException("memberInfo");
            }

            var customAttributeBuilders = propertyAttributeBuilders
                .GetOrAdd(
                          memberInfo,
                          p =>
                            {
                                var customAttributes = p.GetCustomAttributesData();
                                return CreateCustomAttributeBuilders(customAttributes).ToArray();
                            });

            foreach (var attribute in customAttributeBuilders)
            {
                propertyBuilder.SetCustomAttribute(attribute);
            }
        }

        private static IEnumerable<CustomAttributeBuilder> CreateCustomAttributeBuilders(IEnumerable<CustomAttributeData> customAttributes)
        {
            if (customAttributes == null)
            {
                throw new ArgumentNullException("customAttributes");
            }

            var attributeBuilders = customAttributes
                .Select(
                        x =>
                            {
                                var namedArguments = x.NamedArguments ?? new List<CustomAttributeNamedArgument>();
                                var properties = namedArguments.Select(a => a.MemberInfo).OfType<PropertyInfo>().ToArray();
                                var values = namedArguments.Select(a => a.TypedValue.Value).ToArray();
                                var constructorArgs = x.ConstructorArguments.Select(a => a.Value).ToArray();
                                var constructor = x.Constructor;
                                return new CustomAttributeBuilder(constructor, constructorArgs, properties, values);
                            });
            return attributeBuilders;
        }

        private static string GetTypeKey(Type sourceType, Dictionary<string, MemberInfo> fields)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException("sourceType");
            }

            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }

            return fields.Aggregate(sourceType.Name, (current, field) => current + (field.Key + field.Value.MemberType));
        }
    }
}