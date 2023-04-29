using System.Net.Sockets;
using System.Net;

namespace Networker.Common;

public class NetworkHelper
{
    // https://stackoverflow.com/questions/6803073/get-local-ip-address
    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}
