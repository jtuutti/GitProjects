using System;
using System.ComponentModel;
using System.Globalization;

namespace MessageBus.Msmq.Helpers
{
    internal static class ConversionHelper
    {
        #region Methods

        public static TTarget ChangeType<TSource, TTarget>(TSource value)
        {
            object returnValue = ChangeType(value, typeof(TTarget));

            return returnValue != null ? (TTarget) returnValue : default(TTarget);
        }

        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null) return null;
            if (conversionType == null) throw new ArgumentNullException("conversionType");
            if (value.GetType() == conversionType) return value;

            if (conversionType.IsGenericType &&
                conversionType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                conversionType.GetGenericArguments()[0].IsEnum)
            {
                conversionType = conversionType.GetGenericArguments()[0];
            }

            if (conversionType.IsEnum)
            {
                var stringValue = value as string;

                return !String.IsNullOrEmpty(stringValue) ?
                            Enum.Parse(conversionType, stringValue) :
                            Enum.ToObject(conversionType, value);
            }

            return CompatibleChangeType(value, conversionType);
        }

        #endregion

        #region Private Methods

        private static object CompatibleChangeType(object value, Type conversionType)
        {
            try
            {
                return Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                var converter = TypeDescriptor.GetConverter(conversionType);

                if (converter.CanConvertFrom(value.GetType()))
                {
                    return converter.ConvertFrom(value);
                }

                throw;
            }
        }

        #endregion
    }
}
