using System.Net.Sockets;
using System.Net;
using Network.Common;
using System.Threading;
using Network.Client;
using UDP_Networker.Server;
using System.ComponentModel.DataAnnotations;

namespace Network.Server;

/*
 * Server and Client connect over TCP
 * Then the server gives the client a port to send to, and the client gives a port for the server to send to.
 * Then we use UDP from there with TCP critical write support
 */

/// <summary>
/// A server to client connection handle. Suport for TCP writes and UDP streaming
/// </summary>
public class Server : IDisposable
{
    public bool AcceptClients { get; set; } = true;
    public bool Disposed { get; private set; } = false;

    //private List<ClientHandle> _clients = new List<ClientHandle>();
    private Dictionary<uint, ClientHandle> _clients = new Dictionary<uint, ClientHandle>();
    private List<ClientHandle> _clientsDisconnected = new List<ClientHandle>();
    private int _clientPort = 5678; // Doing this and just adding one to the port for each client is a really bad ideer
    private uint _clientIDCounter = 0;
    private byte[] data;
    private TcpListener _clientRequestListener = new TcpListener(IPAddress.Any, Consts.LISTENING_PORT);
    private CancellationToken cancellationToken = new CancellationToken();
    private IPEndPoint? _listenIPEndPoint = new IPEndPoint(IPAddress.Any, Consts.LISTENING_PORT);

    public Server()
    {
        Logger.Log("Starting server up", LogWarningLevel.Info);

        data = new byte[2048];

        _clientRequestListener.Start();
        Task.Run(ListenForClient);
    }

    private async void ListenForClient()
    {
        while (true)
        {
            while (!AcceptClients && !Disposed)
                await Task.Delay(1000);

            if (Disposed)
                return;
            TcpClient client;
            try
            {
                client = await _clientRequestListener.AcceptTcpClientAsync(cancellationToken);
            }
            catch (SocketException e)
            {
                Logger.Log(e.Message, LogWarningLevel.Error);
                return;
            }
            HandleClientConnection(client);
        }
    }


    private void HandleClientConnection(TcpClient client)
    {
        if (Disposed)
            return;

        Logger.Log("Server got TCP connection", LogWarningLevel.Info);

        if (ValidateConnectionRequest(client, out var clientIP, out var userName))
        {
            ClientHandle newClient = new ClientHandle(userName, _clientIDCounter++, client, clientIP, _clientPort++);
            _clients.Add(newClient.ID, newClient);
        }
    }

    private unsafe bool ValidateConnectionRequest(TcpClient client, out IPEndPoint clientIP, out string userName)
    {
        Logger.Log("Trying to validate client request", LogWarningLevel.Info);

        clientIP = new IPEndPoint(0, 0);
        userName = "";

        if (!AcceptClients)
        {
            Logger.Log("Not accepting clients", LogWarningLevel.Info);
            client.Dispose();
            return false;
        }

        var stream = client.GetStream();
        stream.ReadTimeout = 1000; // Wait for data for one seconds
        int bytesRead = stream.Read(data, 0, 1024);

        try
        {
            ConnectionRequest request = Serializer.GetObject<ConnectionRequest>(data);

            clientIP = new IPEndPoint(request.IP, request.Port);
            userName = request.Username;

            stream.Write(Consts.CONNECTION_ESTABLISHED);

            Logger.Log($"New client added, UserName: \"{userName}\"", LogWarningLevel.Succes);

            return true;
        }
        catch (Exception e)
        {
            Logger.Log("Unable to connect to client do to error", LogWarningLevel.Warning);
            client.Dispose();
            //throw;
            return false;
        }
    }

    private void HandleDisconnectedClient(ClientHandle client)
    {
        if (client.IsConnected)
            throw new Exception("Handling a non disconnected client as disconnected");

        _clientsDisconnected.Add(client);
        _clients.Remove(client.ID);

        // Log made so its easy to query
        Logger.Log($"Disconnected client:\nID:{client.ID}\nUsername:{client.UserName}", LogWarningLevel.Info);
    }

    private bool CheckClientConnection(ClientHandle client)
    {
        if (client.IsConnected)
        {
            return true;
        }
        else
        {
            HandleDisconnectedClient(client);
            return false;
        }
    }

    public ClientHandle[] GetDisconnectedClients()
    {
        var ClientsDisconnected = _clientsDisconnected.ToArray();
        _clientsDisconnected.Clear();
        return ClientsDisconnected;
    }

    private bool GetPacketFromClient(in ClientHandle client, List<ServerPacket> packets)
    {
        if (!client.IsConnected)
            return false;

        Packet[] safeBytes = client.ReadSafeData();
        Packet[] notsafeBytes = client.ReadUnsafeData();

        foreach (var data in safeBytes)
            packets.Add(new ServerPacket(data, client.ID));

        foreach (var data in notsafeBytes)
            packets.Add(new ServerPacket(data, client.ID));

        return true;
    }

    /// <summary>
    /// Check for disconnects and return recived packets from clients
    /// </summary>
    public ServerPacket[] Update()
    {
        List<ServerPacket> packets = new List<ServerPacket>();
        var tempClients = _clients.Values;
        foreach (ClientHandle clientHandle in tempClients)
        {
            uint clientID = clientHandle.ID;

            if (!GetPacketFromClient(clientHandle, packets))
                HandleDisconnectedClient(clientHandle);
        }
        return packets.ToArray();
    }

    /// <summary>
    /// Not used for sending data. Should be removed so user cant send data through ClientHandles, wich is the servers job.
    /// </summary>
    public ClientHandle[] GetCurrentClientHandles()
    {
        return _clients.Values.ToArray();
    }

    public void SendPacketToAllClients(Packet packet)
    {
        var tempClients = _clients.Values;
        foreach (ClientHandle clientHandle in tempClients)
        {
            if (!CheckClientConnection(clientHandle))
                continue;

            clientHandle.SendPacket(packet);
        }
    }

    public void SendDataToAllClients<Data_Type>(Data_Type data, bool isSafe = true)
    {
        var tempClients = _clients.Values;
        foreach (ClientHandle clientHandle in tempClients)
        {
            if (!CheckClientConnection(clientHandle))
                continue;

            clientHandle.SendPacket(Serializer.GetPacket(data).ChangeSafty(isSafe));
        }
    }

    public bool SendPacketToClient(uint ID, Packet packet)
    {
        ClientHandle client = _clients[ID];
        if (!CheckClientConnection(client))
            return false;

        client.SendPacket(packet);

        return true;
    }

    public bool SendDataToClient<Data_Type>(uint ID, Data_Type data, bool isSafe = true)
    {
        ClientHandle client = _clients[ID];
        if (!CheckClientConnection(client))
            return false;

        client.SendPacket(Serializer.GetPacket(data).ChangeSafty(isSafe));

        return true;
    }

    public void Dispose()
    {
        Disposed = true;

        _clientRequestListener.Stop();

        foreach (ClientHandle clientHandle in _clients.Values)
            clientHandle.Dispose();

        Disposed = true;
    }
}