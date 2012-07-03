using System;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;

namespace RestFoundation
{
    [Serializable]
    public class HttpResponseException : Exception
    {
        public HttpStatusCode StatusCode { get; protected set; }
        public string StatusDescription { get; protected set; }

        public HttpResponseException(HttpStatusCode statusCode) :
            this(statusCode, String.Empty)
        {
        }

        public HttpResponseException(HttpStatusCode statusCode, string statusDescription) :
            base(String.Format(CultureInfo.InvariantCulture, "Http Status Exception: ({0}) {1}", (int) statusCode, statusDescription ?? String.Empty))
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription ?? String.Empty;
        }

        protected HttpResponseException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            StatusCode = (HttpStatusCode) info.GetInt32("statusCode");
            StatusDescription = info.GetString("statusDescription");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("statusCode", (int) StatusCode);
            info.AddValue("statusDescription", StatusDescription);
        }
    }
}
