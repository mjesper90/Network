using System.Net.Sockets;
using System.Net;
using UDP_Networker.Common;

namespace UDP_Networker.Server;

public class UDP_Server : IDisposable
{
    private List<UDP_ClientHandle> _clients = new List<UDP_ClientHandle>();
    private uint _clientIDCounter = 0;
    private uint _packetIDCounter = 0;
    private UdpClient _reciver = new UdpClient(Consts.LISTENING_PORT);
    private IPEndPoint _listenIPEndPoint = new IPEndPoint(IPAddress.Any, Consts.LISTENING_PORT);

    public bool AcceptClients { get; set; } = true;
    public bool Disposed { get; private set; } = false;

    public UDP_Server()
    {
        Task.Run(HandleClients);
    }

    private void HandleClients()
    {
        // Listen for clients
        _reciver.BeginReceive(HandleDataRecived, null);
    }

    private async void HandleDataRecived(IAsyncResult ar)
    {
        byte[] data = _reciver.EndReceive(ar, ref _listenIPEndPoint);

        if (AcceptClients)
        {
            // Now handle the data we have gotten from client

            throw new NotImplementedException();
        }

        while (!AcceptClients)
            await Task.Delay(1000);

        _reciver.BeginReceive(HandleDataRecived, null);
    }

    public Packet[] HandleData()
    {
        List<Packet> packets = new List<Packet>();

        for (int i = 0; i < _clients.Count; i++)
        {
            UDP_ClientHandle client = _clients[i];

            uint clientID = client.ID;
            uint packetID = _packetIDCounter++;

            throw new NotImplementedException();
        }

        return packets.ToArray();
    }

    public UDP_ClientHandle[] GetCurrentClients()
    {
        return _clients.ToArray();
    }

    public void Dispose()
    {
        throw new NotImplementedException();

        Disposed = true;
    }
}