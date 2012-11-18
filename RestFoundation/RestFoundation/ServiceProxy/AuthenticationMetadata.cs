namespace RestFoundation.ServiceProxy
{
    public sealed class AuthenticationMetadata
    {
        public AuthenticationType Type { get; set; }
        public string DefaultUserName { get; set; }
        public string RelativeUrlToMatch { get; set; }
    }
}