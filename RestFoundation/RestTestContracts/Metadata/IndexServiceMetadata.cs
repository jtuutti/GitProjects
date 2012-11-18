using RestFoundation.Runtime;
using RestFoundation.ServiceProxy;
using RestTestContracts.Resources;

namespace RestTestContracts.Metadata
{
    public class IndexServiceMetadata : ProxyMetadata<IIndexService>
    {
        public override void Initialize()
        {
            SetServiceAuthentication(AuthenticationType.Digest, "admin", "~/async");

            MarkOperationHidden(x => x.Feed(null));
            MarkOperationHidden(x => x.FileDownload());
            MarkOperationHidden(x => x.FileUpload(null));

            SetOperationDescription(x => x.GetAll(), "Gets all resources of type 'Index'");
            SetOperationRequestResourceBuilder(x => x.GetAll(), CreatePersonArrayExample(), XmlSchemaGenerator.Generate<Person[]>());
        }

        private static Person[] CreatePersonArrayExample()
        {
            return new[]
            {
                new Person
                {
                    Name = "Joe Bloe",
                    Age = 41,
                    Values = new[] { "01/21/1971" }
                },
                new Person
                {
                    Name = "Mike Star",
                    Age = 21,
                    Values = new[] { "10/02/1991" }
                },
                new Person
                {
                    Name = "Beth Sue",
                    Age = 33
                }
            };
        }
    }
}
