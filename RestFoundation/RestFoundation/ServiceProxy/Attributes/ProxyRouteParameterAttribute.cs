﻿using System;

namespace RestFoundation.ServiceProxy.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class ProxyRouteParameterAttribute : Attribute
    {
        public ProxyRouteParameterAttribute(object exampleValue)
        {
            if (exampleValue == null) throw new ArgumentNullException("exampleValue");

            ExampleValue = exampleValue;
        }

        public string AllowedValues { get; set; }
        internal object ExampleValue { get; private set; }
    }
}