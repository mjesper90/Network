using System.Net.Sockets;
using UDP_Development;
using Network;
using Network.Client;
using Network.Common;
using Network.Server;

bool Running = true;

Server server = new Server();
Client client = new Client("Just somebody that i use to know");

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