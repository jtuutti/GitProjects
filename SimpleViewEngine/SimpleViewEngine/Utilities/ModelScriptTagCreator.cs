using System.Text;
using System.Web.Helpers;

namespace SimpleViewEngine.Utilities
{
    internal static class ModelScriptTagCreator
    {
        public static string Create(string modelPropertyName, object model)
        {
            string serializedModel = Json.Encode(model);

            var modelBuilder = new StringBuilder();
            modelBuilder.AppendLine("<script type=\"text/javascript\">");
            modelBuilder.Append("window.").Append(modelPropertyName).Append(" = ").Append(serializedModel).AppendLine(";");
            modelBuilder.Append("</script>");

            return modelBuilder.ToString();
        }
    }
}
