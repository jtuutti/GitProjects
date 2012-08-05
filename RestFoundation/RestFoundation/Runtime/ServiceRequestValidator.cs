using System;
using System.Globalization;
using System.Web;
using System.Web.Util;

namespace RestFoundation.Runtime
{
    /// <summary>
    /// Represents an HTTP request validator for the REST Foundation services and web pages.
    /// </summary>
    public sealed class ServiceRequestValidator : RequestValidator
    {
        internal const string UnvalidatedHandlerKey = "_ignoreRequest";

        /// <summary>
        /// Validates a string that contains HTTP request data.
        /// </summary>
        /// <returns>
        /// true if the string to be validated is valid; otherwise, false.
        /// </returns>
        /// <param name="context">The context of the current request.</param>
        /// <param name="value">The HTTP request data to validate.</param>
        /// <param name="requestValidationSource">
        /// An enumeration that represents the source of request data that is being validated. The following are possible values
        /// for the enumeration:QueryStringForm CookiesFilesRawUrlPathPathInfoHeaders
        /// </param>
        /// <param name="collectionKey">
        /// The key in the request collection of the item to validate. This parameter is optional. This parameter is used if the
        /// data to validate is obtained from a collection. If the data to validate is not from a collection,
        /// <paramref name="collectionKey"/> can be null.
        /// </param>
        /// <param name="validationFailureIndex">
        /// When this method returns, indicates the zero-based starting point of the problematic or invalid text in the request
        /// collection. This parameter is passed uninitialized.
        /// </param>
        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if ((context.Items[UnvalidatedHandlerKey] as string) == Boolean.TrueString)
            {
                validationFailureIndex = 0;
                return true;
            }

            string serviceProxyRelativeUrl = Rest.Active.ServiceProxyRelativeUrl;

            if (!String.IsNullOrEmpty(serviceProxyRelativeUrl) &&
                context.Request.AppRelativeCurrentExecutionFilePath.Equals(String.Format(CultureInfo.InvariantCulture,
                                                                                         "~/{0}/proxy", serviceProxyRelativeUrl),
                                                                                         StringComparison.OrdinalIgnoreCase))
            {
                validationFailureIndex = 0;
                return true;
            }

            return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
        }
    }
}
