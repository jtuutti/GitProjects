using System;
using System.ComponentModel;
using System.Globalization;

namespace RestFoundation.Runtime
{
    public static class SafeConvert
    {
        public static object ChangeType(object value, Type conversionType)
        {
            if (value == null) return null;
            if (conversionType == null) throw new ArgumentNullException("conversionType");
            if (value.GetType() == conversionType) return value;

            var stringValue = value as string;

            if (conversionType.IsEnum && !String.IsNullOrEmpty(stringValue))
            {
                return Enum.Parse(conversionType, stringValue);
            }

            return CompatibleChangeType(value, conversionType);
        }

        private static object CompatibleChangeType(object value, Type conversionType)
        {
            try
            {
                return Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(conversionType);

                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        return converter.ConvertFrom(value);
                    }
                }
                catch (Exception)
                {
                }

                return null;
            }
        }
    }
}
