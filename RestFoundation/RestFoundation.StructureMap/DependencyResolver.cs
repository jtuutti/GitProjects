using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RestFoundation.DependencyInjection;
using RestFoundation.StructureMap.Properties;
using StructureMap;

namespace RestFoundation.StructureMap
{
    public sealed class DependencyResolver : IDependencyResolver, IDisposable
    {
        public readonly IContainer m_container;

        public DependencyResolver(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            m_container = container;
        }

        public object Resolve(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            try
            {
                return (type.IsInterface || type.IsAbstract) ? m_container.TryGetInstance(type) : m_container.GetInstance(type);
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public T Resolve<T>()
        {
            Type objectType = typeof(T);

            try
            {
                return (objectType.IsInterface || objectType.IsAbstract) ? m_container.TryGetInstance<T>() : m_container.GetInstance<T>();
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public object Resolve(string key, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            try
            {
                return (type.IsInterface || type.IsAbstract) ? m_container.TryGetInstance(type, key) : m_container.GetInstance(type, key);
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public T Resolve<T>(string key)
        {
            Type objectType = typeof(T);

            try
            {
                return (objectType.IsInterface || objectType.IsAbstract) ? m_container.TryGetInstance<T>(key) : m_container.GetInstance<T>(key);
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            try
            {
                return m_container.GetAllInstances(type).Cast<object>();
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            try
            {
                return m_container.GetAllInstances<T>();
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public void BuildUp(object obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");

            try
            {
                m_container.BuildUp(obj);
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyBuildUpError, ex.Message), ex);
            }
        }

        public void Dispose()
        {
            m_container.Dispose();
        }
    }
}
