// <copyright>
// Dmitry Starosta, 2012-2013
// </copyright>
using System;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a result with a custom response action.
    /// </summary>
    public class ResponseResult : IResult
    {
        private readonly Action<IServiceContext> m_responseAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseResult"/> class.
        /// </summary>
        /// <param name="responseAction">The response action.</param>
        public ResponseResult(Action<IServiceContext> responseAction)
        {
            if (responseAction == null)
            {
                throw new ArgumentNullException("responseAction");
            }

            m_responseAction = responseAction;
        }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public virtual void Execute(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            m_responseAction.Invoke(context);
        }
    }
}
