using Network.Common;
using System.Net;
using System.Net.Sockets;

namespace Network.Server;

public class ClientHandle : IDisposable
{
    public string UserName { get; init; }
    public uint ID { get; init; }
    public IPEndPoint EndPointIP { get; init; }
    public int LocalPort { get; init; }

    public bool IsConnected { get => _network?.IsConnected ?? false; }

    private Common.Network _network { get; init; }

    public ClientHandle(string userName, uint iD, TcpClient tcpClient, IPEndPoint endPointIP, int localPort)
    {
        UserName = userName;
        ID = iD;
        EndPointIP = endPointIP;
        LocalPort = localPort;
        _network = new Common.Network(tcpClient, endPointIP, localPort);
    }

    /// <summary>
    /// Read data send by the client over TCP
    /// </summary>
    public Packet[] ReadSafeData()
    {
        return _network.ReadSafeData();
    }

    /// <summary>
    /// Read data packets send by the client over UDP
    /// </summary>
    public Packet[] ReadUnsafeData()
    {
        return _network.ReadUnsafeData();
    }

    /// <summary>
    /// Send data to client over UDP
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
