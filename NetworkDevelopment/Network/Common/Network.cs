using System.Net.Sockets;
using System.Net;

namespace Network.Common;

/// <summary>
/// Used class that makes it easier to read and write to each other over both TCP and UDP
/// </summary>
public class Network : IDisposable
{
    private TcpClient _tcp;
    private byte[] _tcpBuffer;
    private NetworkStream _safeStream;

    private UdpClient _udp;
    private IPEndPoint _endPoint;
    private IPEndPoint? _localRecivePort;

    private List<Packet> _unsafePackets = new List<Packet>();
    private List<Packet> _safePackets = new List<Packet>();

    private bool _recivedDisconnectMessage = false;

    public int TCPBufferSize { get; init; }

    public bool IsConnected => _tcp.Connected;
    public bool IsDisposed { get; private set; } = false;

    public int ReadSafeDataTimeOut { get => _safeStream.ReadTimeout; set => _safeStream.ReadTimeout = value; }
    public int WriteSafeDataTimeOut { get => _safeStream.WriteTimeout; set => _safeStream.WriteTimeout = value; }

    public Network(TcpClient TCPConnection, IPEndPoint endPointIP, int localPort, int TCPBufferSize = 1024 * 1024)
    {
        _tcp = TCPConnection;
        _safeStream = _tcp.GetStream();
        _endPoint = endPointIP;
        _localRecivePort = new IPEndPoint(NetworkHelper.GetLocalIPAddress(), localPort);
        _udp = new UdpClient(_localRecivePort);
        _udp.Connect(_endPoint);

        this.TCPBufferSize = TCPBufferSize;
        _tcpBuffer = new byte[TCPBufferSize];

        Task.Run(StartUDPRecive);
        Task.Run(StartTCPRecive);
    }

    private void StartUDPRecive() => _udp.BeginReceive(ReciveUDPData, null);
    private void StartTCPRecive() => _safeStream.BeginRead(_tcpBuffer, 0, _tcpBuffer.Length, ReciveTCPData, null);

    private void ReciveUDPData(IAsyncResult ar)
    {
        if (IsDisposed)
            return;

        if (!IsConnected)
        {
            Dispose();
            return;
        }

        byte[] data = _udp.EndReceive(ar, ref _localRecivePort);

        _unsafePackets.Add(new(data, data.Length, false));

        _udp.BeginReceive(ReciveUDPData, null);
    }

    private async void ReciveTCPData(IAsyncResult ar)
    {
        if (IsDisposed)
            return;

        if (!IsConnected)
        {
            Dispose();
            return;
        }
        
        int count = _safeStream.EndRead(ar);

        byte[] data = new byte[count];
        Array.Copy(_tcpBuffer, data, count);
        _safePackets.Add(new(data, data.Length, true));

        _safeStream.BeginRead(_tcpBuffer, 0, _tcpBuffer.Length, ReciveTCPData, null);
    }

    private void ThrowIfInvalidUse()
    {
        if (!IsConnected)
            throw new Exception("Can not use a network that is not connected");
        if (IsDisposed)
            throw new Exception("Can not use a network that is disposed");
    }

    public Packet[] ReadSafeData()
    {
        Packet[] data = _safePackets.ToArray();
        _safePackets.Clear();
        return data;
    }

    public Packet[] ReadUnsafeData()
    {
        Packet[] data = _unsafePackets.ToArray();
        _unsafePackets.Clear();
        return data;
    }

    public void SendPacket(Packet packet)
    {
        ThrowIfInvalidUse();

        if (packet.SafePacket)
            _safeStream.Write(packet.Data, 0, packet.Size);
        else
            _udp.Send(packet.Data, packet.Size);
    }

    public void Dispose()
    {
        IsDisposed = true;

        _safeStream.Dispose();
        _tcp.Dispose();
        _udp.Client.Close();
        _udp.Dispose();
    }
}
