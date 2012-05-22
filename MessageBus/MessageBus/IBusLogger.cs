namespace MessageBus
{
    public interface IBusLogger
    {
        void Debug(string info, params object[] args);
        void Error(string info, params object[] args);
        void Info(string info, params object[] args);
        void Warning(string info, params object[] args);
    }
}
