// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation
{
    /// <summary>
    /// Contains common parameter type constaints.
    /// </summary>
    public static class ParameterType
    {
        /// <summary>
        /// Represents any integer number up to 7 digits.
        /// </summary>
        public const string AnyInteger = @"^-?\d{1,7}$";

        /// <summary>
        /// Represents a non-negative integer number up to 7 digits.
        /// </summary>
        public const string UnsignedInteger = @"^\d{1,7}$";

        /// <summary>
        /// Represents any decimal/floating point number.
        /// </summary>
        public const string AnyNumber = @"^-?\d{1,7}(\.\d+)?$";

        /// <summary>
        /// Represents a non-negative decimal/floating point number.
        /// </summary>
        public const string UnsignedNumber = @"^\d+(\.\d+)?$";

        /// <summary>
        /// Represents a boolean true/false value.
        /// </summary>
        public const string Boolean = @"^[Tt][Rr][Uu][Ee]$|^[Ff][Aa][Ll][Ss][Ee]$";

        /// <summary>
        /// Represents a GUID value.
        /// </summary>
        public const string Guid = @"^(\{?([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}?)$";
    }
}
