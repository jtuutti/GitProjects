using System.Text;
using SimpleViewEngine.Serializer;

namespace SimpleViewEngine.Utilities
{
    internal static class ModelScriptTagCreator
    {
        public static string Create(IModelSerializer serializer, string modelPropertyName, object model)
        {
            string serializedModel = serializer.Serialize(model);

            var modelBuilder = new StringBuilder();
            modelBuilder.AppendLine("<script type=\"text/javascript\">");
            modelBuilder.Append("window.").Append(modelPropertyName).Append(" = ").Append(serializedModel).AppendLine(";");
            modelBuilder.Append("</script>");

            return modelBuilder.ToString();
        }
    }
}
