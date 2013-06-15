using System;
using System.Linq;
using System.Threading.Tasks;
using RestFoundation;
using RestFoundation.Collections;
using RestFoundation.Results;

namespace RestTest.Handlers
{
    public class UploadHandler : HttpHandler
    {
        public UploadHandler()
        {
            SetAllowedHttpMethods(HttpMethod.Post);
        }

        public override Task<IResult> ExecuteAsync(IServiceContext context)
        {
            IUploadedFileCollection files = context.Request.Files;

            if (files == null || files.Count == 0)
            {
                return TaskResult.Content("No files", true, "text/plain");
            }

            return TaskResult.Content("Files: " + String.Join(", ", files.Select(f => !String.IsNullOrEmpty(f.Name) ? f.Name : "N/A")), true, "text/plain");
        }
    }
}