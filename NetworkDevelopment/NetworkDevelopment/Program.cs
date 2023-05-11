using System.Net.Sockets;
using UDP_Development;
using Network;
using Network.Client;
using Network.Common;
using Network.Server;

bool Running = true;

Server server = new Server();
Client client = new Client("Yeff");

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

        if (client.IsConnected)
        {
            input = Console.ReadLine()!;
            client.WriteSafeData(Serializer.GetBytes(input));

            Packet[] packets = server.Update();

            foreach (var packet in packets)
            {
                string data = Serializer.GetObject<string>(packet.Data, 0, packet.Size);
                Console.WriteLine("Server recived: " + data);
                data += " Hello World!";
                Console.WriteLine("Server send: " + data);
                foreach (var clientHandle in server.GetClientHandles())
                {
                    byte[] bytes = Serializer.GetBytes(data);
                    clientHandle.WriteSafeData(bytes);

                    byte[] buffer = new byte[1024];
                    Thread.Sleep(2);
                    int count = client.ReadSafeData(buffer);
                    if (count < 1)
                        continue;
                    Console.WriteLine("Client received: " + Serializer.GetObject<string>(buffer, 0, count));
                }
            }


        }
    }
}

server.Dispose();
client.Dispose();

Running = false;