namespace NetworkLib.Common.Logger
{
    public interface ILogNetwork
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}