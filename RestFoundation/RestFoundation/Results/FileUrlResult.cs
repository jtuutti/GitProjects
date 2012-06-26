﻿using System;
using System.IO;
using System.Net;
using RestFoundation.Runtime;

namespace RestFoundation.Results
{
    public class FileUrlResult : IResult
    {
        public FileUrlResult()
        {
            ClearOutput = true;
        }

        public IServiceContext Context { get; set; }
        public IHttpRequest Request { get; set; }
        public IHttpResponse Response { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public bool CacheOutput { get; set; }
        public bool ClearOutput { get; set; }

        public void Execute()
        {
            if (Response == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No HTTP context found");
            }

            if (FileUrl == null)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid file URL provided");
            }

            var file = new FileInfo(Response.MapPath(FileUrl));

            if (!file.Exists)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError, "No valid file URL provided");
            }

            if (ClearOutput)
            {
                Response.Output.Clear();
            }

            if (!String.IsNullOrEmpty(ContentType))
            {
                Response.SetHeader("Content-Type", ContentType);
            }
            else
            {
                string acceptType = Request.GetPreferredAcceptType();

                if (!String.IsNullOrEmpty(acceptType))
                {
                    Response.SetHeader("Content-Type", acceptType);
                }
            }

            if (!String.IsNullOrEmpty(ContentDisposition))
            {
                Response.SetHeader("Content-Disposition", ContentType);
            }

            Response.SetCharsetEncoding(Request.Headers.AcceptCharsetEncoding);

            OutputCompressionManager.FilterResponse(Request, Response);

            if (CacheOutput)
            {
                Response.SetFileDependencies(file.FullName);
            }

            Response.TransmitFile(file.FullName);
        }
    }
}
