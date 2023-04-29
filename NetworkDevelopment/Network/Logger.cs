namespace Network;

/// <summary>
/// Indicator of what type of Log is being pushed
/// </summary>
public enum LogWarningLevel
{
    Empty,
    Debug,
    Info,
    Succes,
    Warning,
    Error
}

public class Log
{
    public string Message;
    public DateTime TimeSent;
    public LogWarningLevel LogWarningLevel;

    public Log(string message, DateTime timeSent, LogWarningLevel logWarningLevel)
    {
        Message = message;
        TimeSent = timeSent;
        LogWarningLevel = logWarningLevel;
    }
}

public static class Logger
{
    private static Queue<Log> _logs;
    private static LogWarningLevel _logWarningLevelFilter;

    public static int LogCount => _logs.Count;

    static Logger()
    {
        _logs = new Queue<Log>();
        _logWarningLevelFilter = LogWarningLevel.Info;
    }

    public static void Log(string Message, LogWarningLevel LogWarningLevel)
    {
        if (LogWarningLevel >= _logWarningLevelFilter)
            _logs.Enqueue(new Log(Message, DateTime.Now, LogWarningLevel));
    }

    public static bool GetLog(out Log log)
    {
        if (_logs.Count > 0)
        {
            log = _logs.Dequeue();
            return true;
        }
        else
        {
            log = new Log("", new DateTime(), LogWarningLevel.Empty);
            return false;
        }
    }

    public static void SetWarningLevel(LogWarningLevel WarningLevel)
    {
        _logWarningLevelFilter = WarningLevel;
    }
}