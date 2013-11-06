using System.Text;
using System.Web.Helpers;

namespace SimpleViewEngine.Utilities
{
    internal static class ModelScriptTagCreator
    {
        public static string Create(object model)
        {
            string serializedModel = Json.Encode(model);

            var modelBuilder = new StringBuilder();
            modelBuilder.AppendLine("<script type=\"text/javascript\">");
            modelBuilder.Append("window.Model = ").Append(serializedModel).AppendLine(";");
            modelBuilder.Append("</script>");

            return modelBuilder.ToString();
        }
    }
}
