using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace Network.Common;

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

    // https://stackoverflow.com/questions/570098/in-c-how-to-check-if-a-tcp-port-is-available
    public static bool IsPortFreeForTCPUse(int port)
    {
        // Evaluate current system tcp connections. This is the same information provided
        // by the netstat command line application, just in .Net strongly-typed object
        // form.  We will look through the list, and if our port we would like to use
        // in our TcpClient is occupied, we will set isAvailable to false.
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

        foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
        {
            if (tcpi.LocalEndPoint.Port == port)
            {
                return false;
            }
        }

        return true;
    }

    public static int GetFreePort(int port)
    {
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

        while (true)
        {
            bool valid = true;

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                return port;
            port++;
        }
    }
}
