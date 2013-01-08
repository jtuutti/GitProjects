// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;

namespace RestFoundation.Runtime
{
    internal static class WebFormsInjectionHelper
    {
        public static void InitializeChildControls(Control control)
        {
            var childControls = control.Controls.OfType<UserControl>();

            foreach (var childControl in childControls)
            {
                InjectControlDependencies(childControl);
                InitializeChildControls(childControl);
            }
        }

        public static void InjectControlDependencies(Control control)
        {
            Type controlType = control.GetType().BaseType;

            if (controlType == null)
            {
                return;
            }

            ConstructorInfo constructor = (from ctor in controlType.GetConstructors()
                                           let parameterLength = ctor.GetParameters().Length
                                           where parameterLength > 0
                                           orderby parameterLength descending
                                           select ctor).FirstOrDefault();

            if (constructor != null)
            {
                CallPageInjectionConstructor(control, constructor);
            }
        }

        private static void CallPageInjectionConstructor(Control control, ConstructorInfo constructor)
        {
            var parameters = from parameter in constructor.GetParameters()
                             let parameterType = parameter.ParameterType
                             select Rest.Configuration.ServiceLocator.GetService(parameterType);

            constructor.Invoke(control, parameters.ToArray());
        }
    }
}
