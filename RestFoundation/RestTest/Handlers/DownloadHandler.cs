using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Results;

namespace RestTest.Handlers
{
    public class DownloadHandler : HttpHandler
    {
        public override Task<IResult> ExecuteAsync(IServiceContext context)
        {
            if (context.Request.Method != HttpMethod.Get)
            {
                return TaskResult.ResponseStatus(HttpStatusCode.MethodNotAllowed, "Method not allowed", new Dictionary<string, string>
                {
                    { context.Response.HeaderNames.Allow, HttpMethod.Get.ToString().ToUpper() }
                });
            }

            string fileName = context.Request.QueryString.TryGet("fileName");

            if (String.IsNullOrWhiteSpace(fileName))
            {
                return TaskResult.BadRequest;
            }

            return TaskResult.Start(() => new FilePathResult
            {
                FilePath = fileName,
                ContentType = "video/x-ms-wmv",
                ContentDisposition = "attachment; filename=movie.avi"
            });
        }
    }
}