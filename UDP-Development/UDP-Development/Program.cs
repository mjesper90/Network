using System.Net.Sockets;
using UDP_Development;
using UDP_Networker.Client;
using UDP_Networker.Common;
using UDP_Networker.Server;

bool Running = true;

UDP_Server server = new UDP_Server();
UDP_Client client = new UDP_Client("Just somebody that i use to know");

Task.Run(() =>
{
    Logger.SetWarningLevel(LogWarningLevel.Debug);
    while (Running)
    {
        while (Logger.GetLog(out var log))
            Logger_Helper.PrintLogToConsole(log);
        Thread.Sleep(1);
    }
});

while (true)
{
    string input = Console.ReadLine()!.ToLower();
    if (input == "q" || input == "quit")
        break;
    if (input == "try")
    {
        client.Connect(new(NetworkHelper.GetLocalIPAddress().Address, Consts.LISTENING_PORT));
    }
}

server.Dispose();
client.Dispose();

Running = false;