using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Networker.Common;

/// <summary>
/// A class for helping client to send a connection request
/// </summary>
[Serializable]
public unsafe struct ConnectionRequest
{
    public long IP;
    public int Port;
    public string UserName;

    public ConnectionRequest(long ip, int port, string userName)
    {
        IP = ip;
        Port = port;
        UserName = userName;
    }
}
