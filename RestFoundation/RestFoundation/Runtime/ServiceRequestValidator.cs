using System;
using System.Web;
using System.Web.Util;

namespace RestFoundation.Runtime
{
    public sealed class ServiceRequestValidator : RequestValidator
    {
        internal const string UnvalidatedHandlerKey = "_validateRequest";

        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            if ((context.Items[UnvalidatedHandlerKey] as string) == Boolean.TrueString)
            {
                validationFailureIndex = 0;
                return true;
            }

            return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
        }
    }
}
