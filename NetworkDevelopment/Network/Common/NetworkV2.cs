using Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Network;

namespace UDP_Networker.Common;

/// <summary>
/// Used class that makes it easier to read and write to each other over both TCP and UDP
/// </summary>
public class NetworkV2 : IDisposable
{
    public bool IsReadyForUse { get => !(IsListeningForClient || IsConnected || IsConnecting || IsDisposed); }
    public bool IsListeningForClient { get; private set; } = false;
    public bool IsConnecting { get; private set; } = false;
    public bool IsConnected { get => _tcp.Connected; }
    public bool IsDisposed { get; private set; } = false;

    private TcpClient _tcp = new TcpClient();
    private NetworkStream _safeStream = (NetworkStream)NetworkStream.Null;
    private UdpClient _udp = new UdpClient();

    public NetworkV2()
    {
    }

    public void ConnectAsync(IPEndPoint endpoint)
    {
        _ = Task.Run(() => Connect(endpoint));
    }

    public void Connect(IPEndPoint endpoint)
    {
        if (!IsReadyForUse)
            throw new Exception("The current Network is not suited for connecting to host due to either being active or disposed");

        IsConnecting = true;

        _tcp.Connect(endpoint);

        if (!_tcp.Connected)
        {
            Logger.Log("Could not connect to Network at the TCP client level", LogWarningLevel.Info);
            IsConnecting = false;
            return;
        }


    }

    public void ListenForNetworkAsync(int port = -1)
    {
        _ = Task.Run(() => ListenForNetwork(port));
    }

    public void ListenForNetwork(int port = -1)
    {
        if (!IsReadyForUse)
            throw new Exception("The current Network is not suited for listening for clients due to either being active or disposed");

        if (port == -1)
            port = NetworkHelper.GetNextFreePort(Consts.SEVER_LISTENING_PORT);

        TcpListener listener = new TcpListener(IPAddress.Any, port);

        while (true)
        {
            listener.Start();
            _tcp = listener.AcceptTcpClient();
            listener.Stop();



        }
    }

    public void Dispose()
    {
        IsDisposed = true;
    }
}



/*
if (IsDisposed)
    throw new Exception("Can not connect a disposed network");
if (IsListeningForClient)
    throw new Exception("Can not connect a Network to an endpoint when listening for other networks");
if (IsConnected)
    throw new Exception("Can not connect a Network that is allready connected");
if (IsConnecting)
    throw new Exception("Can not connect a Newwork that it allready connecting to another Network");
 */