using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Network.Common;

/// <summary>
/// A class for helping client to send a connection request
/// </summary>
[Serializable]
public unsafe struct ConnectionRequest
{
    public IPEndPoint UDPRecivePort;
    public string Username;

    public ConnectionRequest(IPEndPoint endPoint, string username)
    {
        if (!CheckIfValidUsername(username))
            throw new ArgumentException("Invalid username");

        UDPRecivePort = endPoint;
        Username = username;
    }

    public static bool CheckIfValidUsername(string username)
    {
        if (username.Contains('\n'))
            return false;

        return true;
    }
}
