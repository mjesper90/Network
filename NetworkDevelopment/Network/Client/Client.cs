using System.Net;
using System.Net.Sockets;
using Network.Common;

namespace Network.Client;

public enum ClientState
{
    Standby,
    Connecting,
    Connected,
}

public class Client : IDisposable
{
    private IPEndPoint _server;
    private IPAddress _localAdress;

    private Common.Network? _network;

    public ClientState State { get; private set; } = ClientState.Standby;
    public bool Disposed { get; private set; } = false;

    public string UserName;

    public Client(string userName)
    {
        _server = new IPEndPoint(0, 0);
        _localAdress = NetworkHelper.GetLocalIPAddress();

        UserName = userName;
    }

    public void Connect(IPEndPoint serverAdress)
    {
        if (_network is not null)
            if (!_network.IsDisposed)
                _network.Dispose();

        _ = Task.Run(() => InternalConnectToServer(serverAdress));
    }

    private void InternalConnectToServer(IPEndPoint serverAdress)
    {
        if (State != ClientState.Standby)
            throw new Exception("Can not connect to server when the client is allready connected or is currently connecting");

        State = ClientState.Connecting;

        _server = serverAdress;
        TcpClient tcpClient = new TcpClient();
        tcpClient.Connect(_server);

        if (tcpClient.Connected)
        {
            // Send request
            ConnectionRequest cR = new ConnectionRequest(_localAdress.Address, Consts.CLIENT_RECIVE_PORT, UserName);

            byte[] requestData = Serializer.GetData(cR);
            var stream = tcpClient.GetStream();

            stream.Write(requestData);

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
                State = ClientState.Standby;
                return;
            }

            State = ClientState.Connected;
            return;
        }

        Logger.Log("Could not connect to server", LogWarningLevel.Info);
        State = ClientState.Standby;
    }

    private void ThrowIfNotConnected()
    {

    }

    public void SendSafeData(byte[] data)
    {
        if (State != ClientState.Connected)
            throw new Exception("Can not send data when not connected");

    }

    public void Disconnect()
    {

    }

    public void Dispose()
    {
        _network?.Dispose();
        Disposed = true;
    }
}
