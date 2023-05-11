

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
    public int ReadSafeData(byte[] buffer)
    {
        return _network.ReadSafeData(buffer);
    }

    /// <summary>
    /// Read data send by the client over UDP
    /// </summary>
    public int ReadUnsafeData(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Send data to client over TCP
    /// </summary>
    public void WriteSafeData(byte[] buffer, int amount = -1)
    {
        _network.WriteSafeData(buffer, amount);
    }

    /// <summary>
    /// Send data to client over UDP
    /// </summary>
    public int WriteUnsafeData(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _network.Dispose();
    }
}
