

using System.Net.Sockets;
using System.Net;

namespace Network.Common;

/// <summary>
/// Used class that makes it easier to read and write to each other over both TCP and UDP
/// </summary>
public class Network : IDisposable
{
    private static readonly byte[] DisconnectMessage = new byte[] { 0, 1, 2, 3, 123, 123, 123, 123 }; // random

    private TcpClient _tcp;
    private NetworkStream _safeStream;

    private UdpClient _udp;
    private IPEndPoint _ipEndPoint;
    private IPEndPoint _localRecivePort;

    private bool _recivedDisconnectMessage = false;

    public bool IsConnected { get; private set; } = false;
    public bool IsDisposed { get; private set; } = false;

    public int ReadSafeDataTimeOut { get => _safeStream.ReadTimeout; set => _safeStream.ReadTimeout = value; }
    public int WriteSafeDataTimeOut { get => _safeStream.WriteTimeout; set => _safeStream.WriteTimeout = value; }

    public Network(TcpClient TCPConnection, IPEndPoint endPointIP, int localPort)
    {
        _tcp = TCPConnection;
        _safeStream = _tcp.GetStream();
        _udp = new UdpClient();
        _ipEndPoint = endPointIP;
        _localRecivePort = new IPEndPoint(IPAddress.Any, localPort);

        ValidateConnection();
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

    private void SendDisconnectMessage()
    {
        WriteSafeData(DisconnectMessage);
    }

    private bool IsDisconnectMessage(byte[] data, int length)
    {
        if (length != DisconnectMessage.Length)
            return false;

        for (int i = 0; i < DisconnectMessage.Length; i++)
            if (data[i] != DisconnectMessage[i])
                return false;

        return true;
    }

    public int ReadSafeData(byte[] buffer, int amount = -1)
    {
        ThrowIfInvalidUse();

        if (amount == -1)
            amount = buffer.Length;

        int bytesCount = _safeStream.Read(buffer, 0, amount);
        
        if (IsDisconnectMessage(buffer, bytesCount))
        {
            _recivedDisconnectMessage = true;
            Dispose();
            return 0;
        }

        return bytesCount;
    }

    public byte[] ReadUnsafeData()
    {
        ThrowIfInvalidUse();

        return _udp.Receive(ref _localRecivePort);
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
        if (!_recivedDisconnectMessage) // No need to send message back
            SendDisconnectMessage();

        IsDisposed = true;
        IsConnected = false;

        _tcp.Dispose();
        _safeStream.Dispose();
        _udp.Dispose();
    }
}
