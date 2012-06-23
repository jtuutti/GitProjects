using System;

namespace RestFoundation
{
    [Serializable]
    public class Operation
    {
        public string RelativeUrlTemplate { get; set; }
        public string HttpMethod { get; set; }
        public string Description { get; set; }
    }
}
