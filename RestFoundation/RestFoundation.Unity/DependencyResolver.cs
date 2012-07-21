using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Practices.Unity;
using RestFoundation.DependencyInjection;
using RestFoundation.Unity.Properties;

namespace RestFoundation.Unity
{
    public sealed class DependencyResolver : IDependencyResolver, IDisposable
    {
        private readonly IUnityContainer m_container;

        public DependencyResolver(IUnityContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            m_container = container;
        }

        public object Resolve(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            try
            {
                if (!type.IsInterface && !type.IsAbstract)
                {
                    return m_container.Resolve(type);
                }

                if (m_container.IsRegistered(type))
                {
                    return m_container.Resolve(type);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public T Resolve<T>()
        {
            Type type = typeof(T);

            try
            {
                if (!type.IsInterface && !type.IsAbstract)
                {
                    return m_container.Resolve<T>();
                }

                if (m_container.IsRegistered<T>())
                {
                    return m_container.Resolve<T>();
                }

                return default(T);
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
                if (!type.IsInterface && !type.IsAbstract)
                {
                    return m_container.Resolve(type, key);
                }

                if (m_container.IsRegistered(type, key))
                {
                    return m_container.Resolve(type, key);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new DependencyInjectionException(String.Format(CultureInfo.InvariantCulture, Resources.DependencyResolutionError, ex.Message), ex);
            }
        }

        public T Resolve<T>(string key)
        {
            Type type = typeof(T);

            try
            {
                if (!type.IsInterface && !type.IsAbstract)
                {
                    return m_container.Resolve<T>(key);
                }

                if (m_container.IsRegistered<T>(key))
                {
                    return m_container.Resolve<T>(key);
                }

                return default(T);
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
                return m_container.ResolveAll(type);
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
                return m_container.ResolveAll<T>();
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
