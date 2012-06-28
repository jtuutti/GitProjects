using System;
using System.ComponentModel;
using System.Globalization;

namespace RestFoundation.Runtime
{
    public static class SafeConvert
    {
        public static bool TryChangeType(object value, Type conversionType, out object changedValue)
        {
            if (value == null)
            {
                changedValue = null;
                return true;
            }

            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }

            if (value.GetType() == conversionType)
            {
                changedValue = value;
                return true;
            }

            var stringValue = value as string;

            if (conversionType.IsEnum && !String.IsNullOrEmpty(stringValue))
            {
                try
                {
                    changedValue = Enum.Parse(conversionType, stringValue);
                    return true;
                }
                catch (Exception)
                {
                    changedValue = null;
                    return false;
                }
            }

            try
            {
                changedValue = Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);
                return true;
            }
            catch (Exception)
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(conversionType);

                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        changedValue = converter.ConvertFrom(value);
                        return true;
                    }
                }
                catch (Exception)
                {
                }

                changedValue = null;
                return false;
            }
        }
    }
}
