using System;
using RestFoundation;
using RestFoundation.ServiceProxy;

namespace RestTestContracts.Metadata
{
    public class DynamicServiceMetadata : ProxyMetadata<IDynamicService>
    {
        public override void Initialize()
        {
            ForMethod(x => x.Post(DynamicArg(), Arg<int?>(), Arg<IHttpRequest>())).SetRequestResourceExample(new ComplexType
                                                                                   {
                                                                                       Name = "Joe Doe",
                                                                                       Age = 40
                                                                                   })
                                                                                   .SetResponseResourceExample(new ComplexType
                                                                                   {
                                                                                       Id = "2",
                                                                                       Name = "Joe Doe",
                                                                                       Age = 40
                                                                                   })
                                                                                   .SetQueryParameter("id", typeof(string), "2");

        }

        private class ComplexType
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

            public bool ShouldSerializeId()
            {
                return !String.IsNullOrEmpty(Id);
            }
        }
    }
}
