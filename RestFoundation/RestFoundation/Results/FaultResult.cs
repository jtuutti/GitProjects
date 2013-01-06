using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestFoundation.Formatters;
using RestFoundation.Runtime;
using RestFoundation.Validation;

namespace RestFoundation.Results
{
    /// <summary>
    /// Represents a fault result.
    /// </summary>
    public class FaultResult : IResult
    {
        private readonly List<ValidationError> m_errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultResult"/> class.
        /// </summary>
        public FaultResult()
        {
            m_errors = new List<ValidationError>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultResult"/> class with provided additional
        /// validation errors.
        /// </summary>
        /// <param name="errors">A sequence of valdiation errors</param>
        public FaultResult(params ValidationError[] errors) : this()
        {
            if (errors == null)
            {
                throw new ArgumentNullException("errors");
            }

            m_errors.AddRange(errors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultResult"/> class with provided additional
        /// validation errors.
        /// </summary>
        /// <param name="errors">A sequence of valdiation errors</param>
        public FaultResult(IEnumerable<ValidationError> errors) : this()
        {
            if (errors == null)
            {
                throw new ArgumentNullException("errors");
            }

            m_errors.AddRange(errors);
        }

        /// <summary>
        /// Executes the result against the provided service context.
        /// </summary>
        /// <param name="context">The service context.</param>
        public void Execute(IServiceContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.Output.Clear();
            context.Response.SetStatus(HttpStatusCode.BadRequest, RestResources.ResourceValidationFailed);

            if (context.Request.Method != HttpMethod.Post && context.Request.Method != HttpMethod.Put && context.Request.Method != HttpMethod.Patch)
            {
                return;
            }

            context.GetHttpContext().Response.TrySkipIisCustomErrors = true;

            if (context.Request.ResourceState == null || context.Request.ResourceState.IsValid)
            {
                return;
            }

            m_errors.AddRange(context.Request.ResourceState);

            IMediaTypeFormatter formatter = GetMediaTypeFormatter(context.Request);

            if (formatter == null)
            {
                return;
            }

            ExecuteFaultResult(context, formatter, GenerateFaultCollection());
        }

        private static IMediaTypeFormatter GetMediaTypeFormatter(IHttpRequest request)
        {
            var contentNegotiator = Rest.Configuration.ServiceLocator.GetService<IContentNegotiator>();

            return MediaTypeFormatterRegistry.GetFormatter(contentNegotiator.GetPreferredMediaType(request));
        }

        private static void ExecuteFaultResult(IServiceContext context, IMediaTypeFormatter formatter, FaultCollection faultCollection)
        {
            IResult result = formatter.FormatResponse(context, faultCollection.GetType(), faultCollection);

            if (result != null)
            {
                result.Execute(context);
            }
        }

        private FaultCollection GenerateFaultCollection()
        {
            return new FaultCollection
            {
                General = m_errors.Where(e => String.IsNullOrEmpty(e.PropertyName)).Select(e => new Fault
                {
                    PropertyName = e.PropertyName,
                    Message = e.Message
                }).ToArray(),
                Resource = m_errors.Where(e => !String.IsNullOrEmpty(e.PropertyName)).Select(e => new Fault
                {
                    PropertyName = e.PropertyName,
                    Message = e.Message
                }).ToArray()
            };
        }
    }
}
