

using System.Net;
using System.Net.Sockets;

namespace UDP_Networker.Server;

public class UDP_ClientHandle : IDisposable
{
    public string UserName;
    public uint ID;
    public IPEndPoint EndPointIP;
    public int LocalPort;
    private UdpClient _reciver;

    public UDP_ClientHandle(string userName, uint iD, IPEndPoint endPointIP, int localPort)
    {
        UserName = userName;
        ID = iD;
        EndPointIP = endPointIP;
        LocalPort = localPort;
        _reciver = new UdpClient(LocalPort);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
