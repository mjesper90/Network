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
    public long IP;
    public int Port;
    public string Username;

    public ConnectionRequest(long ip, int port, string username)
    {
        if (!CheckIfValidUsername(username))
            throw new ArgumentException("Invalid username");

        IP = ip;
        Port = port;
        Username = username;
    }

    public static bool CheckIfValidUsername(string username)
    {
        if (username.Contains('\n'))
            return false;

        return true;
    }
}
