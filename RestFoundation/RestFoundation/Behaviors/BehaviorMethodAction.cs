﻿// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Defines an action before a service method gets executed.
    /// </summary>
    public enum BehaviorMethodAction
    {
        /// <summary>
        /// Continue executing the method.
        /// </summary>
        Execute,

        /// <summary>
        /// Stop executing the method.
        /// </summary>
        Stop
    }
}
