

using System.Net;
using System.Net.Sockets;

namespace Network.Server;

public class ClientHandle : IDisposable
{
    public string UserName;
    public uint ID;
    public IPEndPoint EndPointIP;
    public int LocalPort;
    private TcpClient _TCPConnection;
    private UdpClient _UDPConnection;

    private Common.Network _network;
    public bool IsConnected { get => _network?.IsConnected ?? false; }

    public ClientHandle(string userName, uint iD, TcpClient TCPConnection, IPEndPoint endPointIP, int localPort)
    {
        UserName = userName;
        ID = iD;
        EndPointIP = endPointIP;
        LocalPort = localPort;
        _TCPConnection = TCPConnection;
        _UDPConnection = new UdpClient(LocalPort);


    }

    public int ReadSafeData(byte[] buffer)
    {
        throw new NotImplementedException();
    }
    public int ReadUnsafeData(byte[] buffer)
    {
        throw new NotImplementedException();
    }
    public int WriteSafeData(byte[] buffer)
    {
        throw new NotImplementedException();
    }
    public int WriteUnsafeData(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _UDPConnection.Dispose();
    }
}
