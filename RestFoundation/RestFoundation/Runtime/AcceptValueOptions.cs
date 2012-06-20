﻿namespace RestFoundation.Runtime
{
    /// <summary>
    /// Indicates how the accepted values will be searched for the preferred value.
    /// </summary>
    public enum AcceptValueOptions
    {
        /// <summary>
        /// Allow unknown values if the wildcard support is allowed
        /// </summary>
        None,

        /// <summary>
        /// Deny unknown values ignoring the wildcard support, if applicable
        /// </summary>
        IgnoreWildcards
    }
}
