using System.Net;
using System.Net.Sockets;
using UDP_Networker.Common;

namespace UDP_Networker.Client;

public enum UDP_ClientState
{
    Standby,
    Connecting,
    Connected,
}

public class UDP_Client : IDisposable
{
    private TcpClient _tcpClient;
    private UdpClient _client;
    private IPEndPoint _server;
    private IPAddress _localAdress;

    public UDP_ClientState State { get; private set; } = UDP_ClientState.Standby;
    public bool Disposed { get; private set; } = false;

    public string UserName;

    public UDP_Client(string userName)
    {
        _tcpClient = new TcpClient();
        _client = new UdpClient();
        _server = new IPEndPoint(0, 0);
        _localAdress = NetworkHelper.GetLocalIPAddress();

        UserName = userName;
    }

    private void InternalConnectToServer(IPEndPoint serverAdress)
    {
        if (State != UDP_ClientState.Standby)
            throw new Exception("Can not connect to server when the client is allready connected or is currently connecting");

        State = UDP_ClientState.Connecting;

        _server = serverAdress;
        _tcpClient.Connect(_server);

        if (_tcpClient.Connected)   
        {
            // Send request
            ConnectionRequest cR = new ConnectionRequest(_localAdress.Address, Consts.CLIENT_RECIVE_PORT, UserName);

            byte[] requestData = Serializer.GetData(cR);
            var stream = _tcpClient.GetStream();

            stream.Write(requestData);

            // Check response
            byte[] buffer = new byte[Consts.CONNECTION_ESTABLISHED.Length];
            stream.ReadTimeout = 1000;
            int bytesRead = stream.Read(buffer, 0, Consts.CONNECTION_ESTABLISHED.Length);

            bool Failed = bytesRead != Consts.CONNECTION_ESTABLISHED.Length;
            for (int i = 0; i < buffer.Length; i++)
                if (buffer[i] != Consts.CONNECTION_ESTABLISHED[i])
                    Failed = true;

            _tcpClient.Close(); // Done with tcp

            if (Failed)
            {
                Logger.Log("Could not establish connection to server", LogWarningLevel.Info);
                State = UDP_ClientState.Standby;
                return;
            }

            State = UDP_ClientState.Connected;
            return;
        }

        Logger.Log("Could not connect to server", LogWarningLevel.Info);
        State = UDP_ClientState.Standby;
    }

    public void Connect(IPEndPoint serverAdress)
    {
        _ = Task.Run(() => InternalConnectToServer(serverAdress));
    }

    public void SendData(byte[] data)
    {
        if (State != UDP_ClientState.Connected)
            throw new Exception("Can not send data when not connected");

    }

    public void Disconnect()
    {

    }

    public void Dispose()
    {
        Disposed = true;
        _tcpClient.Dispose();
        _client.Dispose();
    }
}
