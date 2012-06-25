using System;
using System.Net;

namespace RestFoundation.Runtime
{
    public class ResultFactory : IResultFactory
    {
        private readonly IHttpRequest m_request;

        public ResultFactory(IHttpRequest request)
        {
            if (request == null) throw new ArgumentNullException("request");

            m_request = request;
        }

        public IResult Create(object returnedObj)
        {
            if (returnedObj == null)
            {
                return null;
            }

            var result = returnedObj as IResult;

            if (result != null)
            {
                return result;
            }

            return CreateSerializerResult(returnedObj);
        }

        private IResult CreateSerializerResult(object returnedObj)
        {
            string acceptedType = m_request.GetPreferredAcceptType();

            if (String.Equals("application/json", acceptedType, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Json(returnedObj);
            }

            if (String.Equals("application/xml", acceptedType, StringComparison.OrdinalIgnoreCase) ||
                String.Equals("text/xml", acceptedType, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Xml(returnedObj);
            }

            throw new HttpResponseException(HttpStatusCode.NotAcceptable, "No supported content type was provided in the Accept or the Content-Type header");
        }
    }
}
