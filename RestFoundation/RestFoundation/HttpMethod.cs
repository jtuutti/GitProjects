﻿// <copyright>
// Dmitry Starosta, 2012-2014
// </copyright>

using System.ComponentModel;

namespace RestFoundation
{
    /// <summary>
    /// Defines HTTP methods supported by services.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        /// GET
        /// </summary>
        Get,
        
        /// <summary>
        /// POST
        /// </summary>
        Post,
        
        /// <summary>
        /// PUT
        /// </summary>
        Put,
        
        /// <summary>
        /// PATCH
        /// </summary>
        Patch,
        
        /// <summary>
        /// DELETE
        /// </summary>
        Delete,

        /// <summary>
        /// HEAD
        /// </summary>
        Head,

        /// <summary>
        /// OPTIONS
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Options,

        /// <summary>
        /// TRACE
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        Trace
    }
}
