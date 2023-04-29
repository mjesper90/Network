using System.Net.Sockets;
using System.Net;
using UDP_Networker.Common;

namespace UDP_Networker.Server;

/*
 * Server and Client connect over TCP
 * Then the server gives the client a port to send to, and the client gives a port for the server to send to.
 * Then we use UDP from there
 */

public class UDP_Server : IDisposable
{
    private List<UDP_ClientHandle> _clients = new List<UDP_ClientHandle>();
    private uint _clientIDCounter = 0;
    private uint _packetIDCounter = 0;
    private byte[] data;
    private TcpListener _clientRequestListener = new TcpListener(IPAddress.Any, Consts.LISTENING_PORT);
    private IPEndPoint? _listenIPEndPoint = new IPEndPoint(IPAddress.Any, Consts.LISTENING_PORT);

    public bool AcceptClients { get; set; } = true;
    public bool Disposed { get; private set; } = false;

    public UDP_Server()
    {
        data = new byte[ConnectionRequest.SizeInBytes];

        _clientRequestListener.Start();
        _clientRequestListener.BeginAcceptSocket(HandleClientConnection, null);
    }

    private async void HandleClientConnection(IAsyncResult ar)
    {
        TcpClient clientSocket = _clientRequestListener.EndAcceptTcpClient(ar);

        Logger.Log("Trying to accept client request", LogWarningLevel.Info);

        if (ValidateConnectionRequest(clientSocket, out var clientIP, out var userName))
        {
            UDP_ClientHandle newClient = new UDP_ClientHandle(userName, _clientIDCounter++, clientIP, 5678);
            _clients.Add(newClient);
        }

        while (!AcceptClients)
            await Task.Delay(1000);

        _clientRequestListener.BeginAcceptSocket(HandleClientConnection, null);
    }

    private unsafe bool ValidateConnectionRequest(TcpClient clientSocket, out IPEndPoint clientIP, out string userName)
    {
        if (!AcceptClients)
        {
            clientSocket.Close();
            clientIP = new IPEndPoint(0, 0);
            userName = "";
            return false;
        }

        var stream = clientSocket.GetStream();
        stream.Read(data, 0, ConnectionRequest.SizeInBytes);

        try
        {
            ConnectionRequest request = (ConnectionRequest)Serializer.DeSerialize(data);

            clientIP = new IPEndPoint( request.IP, request.Port);
            userName = request.UserName;

            return true;
        }
        catch (Exception)
        {

            Logger.Log("Unable to connect to client", LogWarningLevel.Info);
            clientSocket.Close();
            clientIP = new IPEndPoint(0, 0);
            userName = "";
            return false;
        }
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

    public UDP_ClientHandle[] GetCurrentClientHandles()
    {
        return _clients.ToArray();
    }

    public void Dispose()
    {
        throw new NotImplementedException();

        Disposed = true;
    }
}