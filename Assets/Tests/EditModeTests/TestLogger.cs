using NetworkLib.Common.Logger;

public class TestLogger : ILogNetwork
{
    public void Log(string message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void LogError(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public void LogWarning(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
}