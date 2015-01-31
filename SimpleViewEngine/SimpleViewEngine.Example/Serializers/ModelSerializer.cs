using Newtonsoft.Json;
using SimpleViewEngine.Serializer;

namespace SimpleViewEngine.Example.Serializers
{
    public class ModelSerializer : IModelSerializer
    {
        public string Serialize(object model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
