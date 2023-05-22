﻿using System.Net;
using System.Net.Sockets;
using Network.Common;

namespace Network.Client;

public class Client : IDisposable
{
    private IPEndPoint _server;
    private IPAddress _localAdress;

    private Common.Network? _network;

    public bool IsConnecting { get; private set; } = false;
    public bool IsConnected { get => _network?.IsConnected ?? false; }
    public bool Disposed { get; private set; } = false;

    public string UserName;

    public Client(string userName)
    {
        _server = new IPEndPoint(0, 0);
        _localAdress = NetworkHelper.GetLocalIPAddress();

        UserName = userName;
    }

    public void ConnectAsync(IPEndPoint serverAdress)
    {
        Disconnect();

        _ = Task.Run(() => InternalConnectToServer(serverAdress));
    }

    public /*async*/ void Connect(IPEndPoint serverAdress)
    {
        Disconnect();

        //await Task.Run(() => InternalConnectToServer(serverAdress));
        InternalConnectToServer(serverAdress);
    }

    private void InternalConnectToServer(IPEndPoint serverAdress)
    {
        if (IsConnected || IsConnecting)
            throw new Exception("Can not connect to server when the client is allready connected or is currently connecting");
        IsConnecting = true;

        _server = serverAdress;
        TcpClient tcpClient = new TcpClient();
        tcpClient.Connect(_server);

        if (tcpClient.Connected)
        {
            // Send request
            ConnectionRequest cR = new ConnectionRequest(_localAdress.Address, Consts.CLIENT_RECIVE_PORT, UserName);

            byte[] requestData = Serializer.GetBytes(cR);
            var stream = tcpClient.GetStream();

            stream.Write(requestData);

            // Check response
            byte[] buffer = new byte[Consts.CONNECTION_ESTABLISHED.Length];
            stream.ReadTimeout = 1000;
            int bytesRead = stream.Read(buffer, 0, Consts.CONNECTION_ESTABLISHED.Length);

            bool Failed = bytesRead != Consts.CONNECTION_ESTABLISHED.Length;
            for (int i = 0; i < buffer.Length; i++)
                if (buffer[i] != Consts.CONNECTION_ESTABLISHED[i])
                    Failed = true;

            if (Failed)
            {
                Logger.Log("Could not establish connection to server", LogWarningLevel.Info);
                IsConnecting = false;
                return;
            }

            Logger.Log("Connection established to server", LogWarningLevel.Succes);

            _network = new Common.Network(tcpClient, serverAdress, Consts.CLIENT_RECIVE_PORT);
            IsConnecting = false;
            return;
        }

        Logger.Log("Could not connect to server", LogWarningLevel.Info);
        IsConnecting = false;
    }

    private void ThrowIfNotConnected()
    {
        if (!IsConnected)
            throw new Exception("Can not send data when not connected");
    }

    public void WriteSafeData(byte[] data)
    {
        ThrowIfNotConnected();
        _network!.WriteSafeData(data);
    }

    public void WriteUnsafeData(byte[] data)
    {
        ThrowIfNotConnected();
        _network!.WriteUnsafeData(data);
    }

    public int ReadSafeData(byte[] buffer)
    {
        ThrowIfNotConnected();
        int amount = _network!.ReadSafeData(buffer, buffer.Length);

        if (!_network.IsConnected)
        {
            _network = null;
            return -1;
        }

        return amount;
    }

    public void Disconnect()
    {
        if (_network is null)
            return;
        if (_network.IsDisposed)
            return;

        _network.Dispose();
        _network = null;
    }

    public void Dispose()
    {
        _network?.Dispose();
        Disposed = true;
    }
}