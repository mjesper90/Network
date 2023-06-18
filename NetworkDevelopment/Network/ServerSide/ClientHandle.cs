using Network.Common;
using System.Net;
using System.Net.Sockets;

namespace Network.ServerSide;

public class ClientHandle : IDisposable
{
    public string UserName { get; init; }
    public Guid ID { get; init; }
    public IPEndPoint EndPointIP { get; init; }
    public int LocalPort { get; init; }

    public bool IsConnected { get => _network?.IsConnected ?? false; }

    private Common.Network _network { get; init; }

    public ClientHandle(string userName, Guid iD, TcpClient tcpClient, IPEndPoint endPointIP, int localPort)
    {
        UserName = userName;
        ID = iD;
        EndPointIP = endPointIP;
        LocalPort = localPort;
        _network = new Common.Network(tcpClient, endPointIP, localPort);
    }

    /// <summary>
    /// Read packets send by the client over TCP
    /// </summary>
    public Packet[] ReadSafeData()
    {
        return _network.ReadSafeData();
    }

    /// <summary>
    /// Read packets packets send by the client over UDP
    /// </summary>
    public Packet[] ReadUnsafeData()
    {
        return _network.ReadUnsafeData();
    }

    /// <summary>
    /// Send packet to client
    /// </summary>
    public void SendPacket(Packet packet)
    {
        _network.SendPacket(packet);
    }

    public void Dispose()
    {
        _network.Dispose();
    }
}
