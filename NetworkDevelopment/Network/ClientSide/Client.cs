using System.Net;
using System.Net.Sockets;
using Network.Common;

namespace Network.ClientSide;

public class Client : IDisposable
{
    private IPEndPoint _serverIP;
    private IPEndPoint _localEndpoint;

    private Common.Network? _network;

    public bool IsConnecting { get; private set; } = false;
    public bool IsConnected { get => _network?.IsConnected ?? false; }
    public bool Disposed { get; private set; } = false;

    public string UserName;

    public Client(string userName)
    {
        _serverIP = new IPEndPoint(0, 0);
        _localEndpoint = new IPEndPoint(NetworkHelper.GetLocalIPAddress(), 0);

        UserName = userName;
    }

    public void ConnectAsync(IPEndPoint serverAdress)
    {
        Disconnect();

        _ = Task.Run(() => InternalConnectToServer(serverAdress));
    }

    public  void Connect(IPEndPoint serverAdress)
    {
        Disconnect();

        InternalConnectToServer(serverAdress);
    }

    private void InternalConnectToServer(IPEndPoint serverAdress)
    {
        if (IsConnected || IsConnecting)
            throw new Exception("Can not connect to server when the client is allready connected or is currently connecting");
        IsConnecting = true;

        _serverIP = serverAdress;
        TcpClient tcpClient = new TcpClient();
        try
        {
            tcpClient.Connect(_serverIP);
        }
        catch (Exception)
        {
            Logger.Log("Could not establish connection to server", LogWarningLevel.Info);
            IsConnecting = false;
            return;
        }

        if (tcpClient.Connected)
        {
            // Send request
            int port = NetworkHelper.GetNextFreePort(Consts.CLIENT_UDP_RECIVE_PORT);


            ConnectionRequest cR = new ConnectionRequest(_localEndpoint, UserName);

            Packet requestData = Serializer.GetPacket(cR);
            var stream = tcpClient.GetStream();

            stream.Write(requestData.Data);

            // Check response
            byte[] buffer = new byte[Consts.CONNECTION_ESTABLISHED.Length];
            stream.ReadTimeout = 1000;
            int bytesRead = stream.Read(buffer, 0, Consts.CONNECTION_ESTABLISHED.Length);

            bool Failed = bytesRead != Consts.CONNECTION_ESTABLISHED.Length;
            for (int i = 0; i < buffer.Length; i++)
                if (buffer[i] != Consts.CONNECTION_ESTABLISHED[i])
                    Failed = true;

            if (Failed)
            {
                Logger.Log("Could not establish connection to server", LogWarningLevel.Info);
                IsConnecting = false;
                return;
            }

            Logger.Log("Connection established to server", LogWarningLevel.Succes);

            _network = new Common.Network(tcpClient, serverAdress, port);
            IsConnecting = false;
            return;
        }

        Logger.Log("Could not connect to server", LogWarningLevel.Info);
        IsConnecting = false;
    }

    private void ThrowIfNotConnected()
    {
        if (!IsConnected)
            throw new Exception("Can not send data when not connected");
    }

    public void SendPacket(Packet packet)
    {
        ThrowIfNotConnected();

        _network?.SendPacket(packet);
    }

    public void SendData<Data_Type>(Data_Type data, bool isSafe = true)
    {
        ThrowIfNotConnected();

        _network?.SendPacket(Serializer.GetPacket(data).ChangeSafty(isSafe));
    }

    public Packet[] ReadSafeData()
    {
        ThrowIfNotConnected();

        return _network?.ReadSafeData() ?? throw new Exception("?");
    }

    public Packet[] ReadUnsafeData()
    {
        ThrowIfNotConnected();

        return _network?.ReadUnsafeData() ?? throw new Exception("?");
    }

    public void Disconnect()
    {
        if (_network is null)
            return;
        if (_network.IsDisposed)
            return;

        _network.Dispose();
        _network = null;
    }

    public void Dispose()
    {
        _network?.Dispose();
        Disposed = true;
    }
}
