namespace RestFoundation.Behaviors
{
    /// <summary>
    /// Defines an action in case of a service method exception.
    /// </summary>
    public enum BehaviorExceptionAction
    {
        /// <summary>
        /// Bubble up the exception.
        /// </summary>
        BubbleUp,

        /// <summary>
        /// Handle the exception and stop executing the method.
        /// </summary>
        Handle
    }
}
