

using System.Net.Sockets;
using System.Net;

namespace Network.Common;

/// <summary>
/// Used class that makes it easier to read and write to each other over both TCP and UDP
/// </summary>
public class Network : IDisposable
{
    private TcpClient _tcp;
    private NetworkStream _safeStream;

    private UdpClient _udp;
    private IPEndPoint _ipEndPoint;
    private IPEndPoint _localRecivePort;

    private List<byte[]> _packets = new List<byte[]>();

    private bool _recivedDisconnectMessage = false;

    public bool IsConnected { get; private set; } = false;
    public bool IsDisposed { get; private set; } = false;

    public int ReadSafeDataTimeOut { get => _safeStream.ReadTimeout; set => _safeStream.ReadTimeout = value; }
    public int WriteSafeDataTimeOut { get => _safeStream.WriteTimeout; set => _safeStream.WriteTimeout = value; }

    public Network(TcpClient TCPConnection, IPEndPoint endPointIP, int localPort)
    {
        _tcp = TCPConnection;
        _safeStream = _tcp.GetStream();
        _ipEndPoint = endPointIP;
        _localRecivePort = new IPEndPoint(IPAddress.Any, localPort);
        _udp = new UdpClient(_localRecivePort);

        ValidateConnection();

        Task.Run(ListenOnUDP);
    }

    private void ListenOnUDP()
    {
        IPEndPoint? a = null;

        while (IsConnected)
            _packets.Add(_udp.Receive(ref a));

    }

    private void ValidateConnection()
    {
        if (_tcp.Connected)
            IsConnected = true;
    }

    private void ThrowIfInvalidUse()
    {
        ValidateConnection();

        if (!IsConnected)
            throw new Exception("Can not use a network that is not connected");
        if (IsDisposed)
            throw new Exception("Can not use a network that is disposed");
    }

    public int ReadSafeData(byte[] buffer, int amount = -1)
    {
        ThrowIfInvalidUse();

        if (amount == -1)
            amount = buffer.Length;

        int bytesCount = _safeStream.Read(buffer, 0, amount);

        return bytesCount;
    }

    public byte[][] ReadUnsafeData()
    {
        ThrowIfInvalidUse();
        byte[][] data = _packets.ToArray();
        return data;
    }

    public void WriteSafeData(byte[] buffer, int amount = -1)
    {
        ThrowIfInvalidUse();

        if (amount == -1)
            amount = buffer.Length;

        _safeStream.Write(buffer, 0, amount);
    }

    public void WriteUnsafeData(byte[] buffer, int amount = -1)
    {
        ThrowIfInvalidUse();

        if (amount == -1)
            amount = buffer.Length;

        _udp.Send(buffer, amount);
    }

    public void Dispose()
    {
        IsDisposed = true;
        IsConnected = false;

        _tcp.Dispose();
        _safeStream.Dispose();
        _udp.Client.Close();
        _udp.Dispose();
    }
}
