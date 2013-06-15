using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Results;

namespace RestTest.Handlers
{
    public class DownloadHandler : HttpHandler
    {
        public DownloadHandler()
        {
            SetAllowedHttpMethods(HttpMethod.Get);
        }

        public override Task<IResult> ExecuteAsync(IServiceContext context)
        {
            if (context.Request.Method != HttpMethod.Get)
            {
                return TaskResult.ResponseStatus(HttpStatusCode.MethodNotAllowed, "Method not allowed", new Dictionary<string, string>
                {
                    { context.Response.HeaderNames.Allow, HttpMethod.Get.ToString().ToUpper() }
                });
            }

            string fileName = Params.Get("fileName");

            if (String.IsNullOrWhiteSpace(fileName))
            {
                return TaskResult.BadRequest;
            }

            return TaskResult.Start(() => new FileResult
            {
                FilePath = fileName,
                ContentType = "video/x-ms-wmv",
                ContentDisposition = "attachment; filename=" + Path.GetFileName(fileName)
            });
        }
    }
}
