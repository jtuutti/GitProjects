namespace RestFoundation
{
    /// <summary>
    /// Represents a cache item priority.
    /// </summary>
    public enum CachePriority
    {
        /// <summary>
        /// The default priority.
        /// </summary>
        Normal,

        /// <summary>
        /// Low priority. Low priority items are removed from cache first in
        /// low memory situations.
        /// </summary>
        Low,

        /// <summary>
        /// High priority. High priority items are retained the longest in
        /// low memory situations.
        /// </summary>
        High
    }
}
