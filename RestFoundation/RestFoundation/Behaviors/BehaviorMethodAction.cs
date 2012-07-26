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
