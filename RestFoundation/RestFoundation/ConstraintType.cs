// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation
{
    /// <summary>
    /// Contains common parameter type constaints.
    /// </summary>
    public static class ConstraintType
    {
        /// <summary>
        /// Represents an string containing English letters.
        /// </summary>
        public const string Alpha = @"[A-Za-z]+";

        /// <summary>
        /// Represents an string containing English letters, dashes and underscores.
        /// </summary>
        public const string AlphaWithDashesAndUnderscores = @"[A-Za-z\-_]+";

        /// <summary>
        /// Represents an string containing English letters and digits.
        /// </summary>
        public const string AlphaNumeric = @"[A-Za-z0-9]+";

        /// <summary>
        /// Represents an string containing English letters, digits, dashes and underscores.
        /// </summary>
        public const string AlphaNumericWithDashesAndUnderscores = @"[A-Za-z0-9\-_]+";

        /// <summary>
        /// Represents any integer number up to 10 digits.
        /// </summary>
        public const string AnyInteger = @"-?\d{1,10}";

        /// <summary>
        /// Represents a non-negative integer number up to 10 digits.
        /// </summary>
        public const string UnsignedInteger = @"\d{1,10}";

        /// <summary>
        /// Represents any long integer number up to 19 digits.
        /// </summary>
        public const string AnyLong = @"-?\d{1,19}";

        /// <summary>
        /// Represents a non-negative long integer number up to 19 digits.
        /// </summary>
        public const string UnsignedLong = @"\d{1,19}";

        /// <summary>
        /// Represents any decimal/floating point number.
        /// </summary>
        public const string AnyNumber = @"-?\d+(\.\d+)?";

        /// <summary>
        /// Represents a non-negative decimal/floating point number.
        /// </summary>
        public const string UnsignedNumber = @"\d+(\.\d+)$";

        /// <summary>
        /// Represents a boolean true/false value.
        /// </summary>
        public const string Boolean = @"[Tt][Rr][Uu][Ee]$|^[Ff][Aa][Ll][Ss][Ee]";

        /// <summary>
        /// Represents a GUID value.
        /// </summary>
        public const string Guid = @"(\{?([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}?)";
    }
}
