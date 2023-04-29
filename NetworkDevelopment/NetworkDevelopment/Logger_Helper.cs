using Network;

namespace UDP_Development;

public class Logger_Helper
{
    public static void PrintLogToConsole(Log log)
    {
        switch (log.LogWarningLevel)
        {
            case LogWarningLevel.Empty:
                Console.ForegroundColor = ConsoleColor.Gray;
                break;
            case LogWarningLevel.Debug:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogWarningLevel.Info:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case LogWarningLevel.Succes:
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case LogWarningLevel.Warning:
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                break;
            case LogWarningLevel.Critical:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogWarningLevel.Error:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                break;
            default:
                break;
        }

        Console.WriteLine($"{{{log.TimeSent.ToString("HH:mm:ss")}}} {log.Message}");

    }

    public static void Test()
    {
        Logger.SetWarningLevel(LogWarningLevel.Empty);
        PrintLogToConsole(new Log("This is a \"Empty\" message", DateTime.Now, LogWarningLevel.Empty));
        PrintLogToConsole(new Log("This is a \"Debug\" message", DateTime.Now, LogWarningLevel.Debug));
        PrintLogToConsole(new Log("This is a \"Info\" message", DateTime.Now, LogWarningLevel.Info));
        PrintLogToConsole(new Log("This is a \"Warning\" message", DateTime.Now, LogWarningLevel.Warning));
        PrintLogToConsole(new Log("This is a \"Critical\" message", DateTime.Now, LogWarningLevel.Critical));
        PrintLogToConsole(new Log("This is a \"Error\" message", DateTime.Now, LogWarningLevel.Error));
        Logger.SetWarningLevel(LogWarningLevel.Info);
    }
}
