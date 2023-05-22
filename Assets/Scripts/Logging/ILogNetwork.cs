namespace NetworkLib.Logger
{
    public interface ILogNetwork
    {
        ILogNetwork Instance();
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}