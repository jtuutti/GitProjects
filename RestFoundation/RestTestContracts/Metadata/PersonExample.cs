using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class PersonExample : IResourceExample
    {
        public object Create()
        {
            return new Person
            {
                Name = "Joe Bloe",
                Age = 41,
                Values = new[] { "01/21/1951" }
            };
        }
    }
}
