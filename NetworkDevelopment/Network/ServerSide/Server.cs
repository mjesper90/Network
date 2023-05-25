using System.Net.Sockets;
using System.Net;
using Network.Common;
using UDP_Networker.ServerSide;

namespace Network.ServerSide;

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

    

    private Dictionary<uint, ClientHandle> _clients = new Dictionary<uint, ClientHandle>();
    private List<ClientHandle> _clientsDisconnected = new List<ClientHandle>();
    private int _clientPort = 5678; // Doing this and just adding one to the port for each client is a really bad ideer

    // Starts at 1 since 0 is an invalid id
    private uint _clientIDCounter = 1;
    private TcpListener _clientRequestListener = new TcpListener(IPAddress.Any, Consts.LISTENING_PORT);
    private CancellationToken cancellationToken = new CancellationToken();
    private IPEndPoint? _listenIPEndPoint = new IPEndPoint(IPAddress.Any, Consts.LISTENING_PORT);

    public Server()
    {
        Logger.Log("Starting server up", LogWarningLevel.Info);
        Logger.Log("Looking for clients on: " + GetListeningIP(), LogWarningLevel.Info);

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

    private bool ValidateConnectionRequest(TcpClient client, out IPEndPoint clientIP, out string userName)
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
        byte[] data = new byte[1024];
        int bytesRead = stream.Read(data, 0, 1024);

        try
        {
            ConnectionRequest request = Serializer.GetObject<ConnectionRequest>(data);

            clientIP = request.UDPRecivePort;
            userName = request.Username;

            stream.Write(Consts.CONNECTION_ESTABLISHED);

            Logger.Log($"New client added, UserName: \"{userName}\"", LogWarningLevel.Succes);

            return true;
        }
        catch (Exception)
        {
            Logger.Log("Unable to connect to client do to error", LogWarningLevel.Warning);
            client.Dispose();

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

    public ClientIdentifier[] GetDisconnectedClients()
    {
        var ClientsDisconnected = _clientsDisconnected.ToArray();
        _clientsDisconnected.Clear();
        return ClientHandleToIdentifier(ClientsDisconnected);
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
    public ClientIdentifier[] GetCurrentClientHandles()
    {
        return ClientHandleToIdentifier(_clients.Values.ToArray());
    }

    public int GetConnectedClientsCount()
    {
        return _clients.Count;
    }

    public void SendDataToAllClients<Data_Type>(Data_Type data, bool isSafe = true)
    {
        SendPacketToAllClients(Serializer.GetPacket(data).ChangeSafty(isSafe));
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

    public bool SendDataToClient<Data_Type>(uint ID, Data_Type data, bool isSafe = true)
    {
        return SendPacketToClient(ID, Serializer.GetPacket(data).ChangeSafty(isSafe));
    }

    public bool SendPacketToClient(uint ID, Packet packet)
    {
        ClientHandle client = _clients[ID];
        if (!CheckClientConnection(client))
            return false;

        client.SendPacket(packet);

        return true;
    }

    private ClientIdentifier[] ClientHandleToIdentifier(ClientHandle[] handles)
    {
        ClientIdentifier[] identifiers = new ClientIdentifier[handles.Length];
        for (int i = 0; i < handles.Length; i++)
            identifiers[i] = new ClientIdentifier(handles[i].UserName, handles[i].ID);
        return identifiers;
    }

    public void Dispose()
    {
        Disposed = true;

        _clientRequestListener.Stop();

        foreach (ClientHandle clientHandle in _clients.Values)
            clientHandle.Dispose();
    }

    // Util

    public IPEndPoint GetListeningIP()
    {
        IPEndPoint e = new IPEndPoint(NetworkHelper.GetLocalIPAddress(), Consts.LISTENING_PORT);
        return e;
    }
}