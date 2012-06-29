using System.Xml.Serialization;

namespace RestFoundation.ServiceProxy
{
    public interface IResourceExample
    {
        object Create();
        XmlSchemas GetSchemas();
    }
}
