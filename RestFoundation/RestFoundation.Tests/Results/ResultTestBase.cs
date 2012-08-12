using System.IO;

namespace RestFoundation.Tests.Results
{
    public abstract class ResultTestBase
    {
        protected IServiceContext Context { get; set; }

        protected string GetResponseOutput()
        {
            if (Context == null)
            {
                return null;
            }

            Context.Response.Output.Stream.Position = 0;

            string output;

            using (var reader = new StreamReader(Context.Response.Output.Stream))
            {
                output = reader.ReadToEnd();
            }

            return output;
        }
    }
}
