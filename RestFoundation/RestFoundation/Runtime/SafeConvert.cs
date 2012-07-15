using System;
using System.ComponentModel;
using System.Globalization;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represent a safe type converter.
    /// </summary>
    public static class SafeConvert
    {
        /// <summary>
        /// Returns a value indicating whether the provided value could be changed to the specified
        /// conversion type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="conversionType">The conversion type.</param>
        /// <param name="changedValue">
        /// The value casted into the conversion type, or null if the type could not be changed.
        /// </param>
        /// <returns>The changed value</returns>
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
