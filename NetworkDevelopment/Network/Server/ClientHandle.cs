

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

    public ClientHandle(string userName, uint iD, TcpClient TCPConnection, IPEndPoint endPointIP, int localPort)
    {
        UserName = userName;
        ID = iD;
        EndPointIP = endPointIP;
        LocalPort = localPort;
        _TCPConnection = TCPConnection;
        _UDPConnection = new UdpClient(LocalPort);


    }

    /// <summary>
    /// Data recived through TCP
    /// </summary>
    /// <returns>Amount of bytes recived</returns>
    public int GetSecuredData(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Data recived through UDP
    /// </summary>
    /// <returns>Amount of bytes recived</returns>
    public int GetStreamedData(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _UDPConnection.Dispose();
    }
}
