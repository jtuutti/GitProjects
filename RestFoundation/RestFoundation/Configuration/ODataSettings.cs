using RestFoundation.Behaviors;

namespace RestFoundation.Configuration
{
    /// <summary>
    /// Contains settings used by OData providers.
    /// </summary>
    public sealed class ODataSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether service methods that return
        /// <see cref="T:System.Linq.IQueryable`1"/> should support OData operations.
        /// </summary>
        public bool DisableIQueryableSupport { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether string comparison operations should
        /// be case insensitive. This setting should only be enabled for Linq-to-Objects
        /// providers. Most database providers do not support this feature and can throw
        /// an exception or perform string comparisons based on the table collation.
        /// </summary>
        public bool CaseInsensitiveStringComparison { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether string case conversion operations should
        /// use an invariant culture. This setting should only be enabled for Linq-to-Objects
        /// providers. Most database providers do not support this feature and can throw
        /// an exception or perform string conversions based on the table collation.
        /// </summary>
        public bool InvariantCultureCaseConversion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the maximum number of results an OData enabled
        /// service should return. By default this value cannot be overridden using a $take
        /// in the query string. To override this behavior, decorate specific services or
        /// action methods with the <see cref="MaxQueryResultsAttribute"/> attribute. Set
        /// this value to 0 (default) if you do not want to limit the number of results
        /// returned.
        /// </summary>
        public int MaxResults { get; set; }
    }
}
