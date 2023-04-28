

using System.Net;

namespace UDP_Networker.Server;

public class UDP_ClientHandle
{
    public string UserName;
    public uint ID;
    public IPEndPoint IPAddress;
    public int Port;

    public UDP_ClientHandle(string userName, uint iD, IPEndPoint iPAddress, int port)
    {
        UserName = userName;
        ID = iD;
        IPAddress = iPAddress;
        Port = port;
    }
}
