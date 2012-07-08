namespace RestFoundation.Collections
{
    public interface IServerVariableCollection : IStringValueCollection
    {
        string ApplicationPoolId { get; }
        string HttpVersion { get; }
        string LocalAddress { get; }
        string RemoteAddress { get; }
        int RemotePort { get; }
        string RemoteUser { get; }
        string ServerName { get; }
        int ServerPort { get; }
        string UserAgent { get; }
    }
}
