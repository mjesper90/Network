using Development;
using Network;
using Network.ClientSide;
using Network.Common;
using Network.ServerSide;

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
            client.SendPacket(Serializer.GetPacket(input));
            //client.SendData(false, Serializer.GetBytes(input));

            //Thread.Sleep(1000);

            ServerPacket[] packets = server.Update();

            foreach (var packet in packets)
            {
                string data = Serializer.GetObject<string>(packet);
                Console.WriteLine("Server recived: " + data);
                data += " Hello World!";
                Console.WriteLine("Server send: " + data);
                Packet bytes = Serializer.GetPacket(data);
                server.SendPacketToClient(packet.ClientID, bytes.ChangeSafty(false));
                Thread.Sleep(2);
                Packet[] bytesRecived = client.ReadSafeData();
                if (bytesRecived.Length != 1)
                    continue;
                if (bytesRecived[0].Size < 1)
                    continue;
                Console.WriteLine("Client received: " + Serializer.GetObject<string>(bytesRecived[0]));
            }
        }
    }
}

server.Dispose();
client.Dispose();

Running = false;