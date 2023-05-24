using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.Logger;

namespace NetworkLib.GameClient
{
    public class Client
    {
        public TcpClient Tcp;
        public Network NetworkHandler;

        private NetworkStream _tcpStream;
        private byte[] _tcpReceiveBuffer;
        private BinaryFormatter _binaryFormatter;

        // Logger
        public static ILogNetwork Log;

        public Client(ILogNetwork log, TcpClient socket)
        {
            _binaryFormatter = new BinaryFormatter();
            NetworkHandler = new Network(this);
            Tcp = socket;
            Log = log;
            Tcp.ReceiveBufferSize = CONSTANTS.BufferSize;
            Tcp.SendBufferSize = CONSTANTS.BufferSize;
            _tcpStream = Tcp.GetStream();
            _tcpReceiveBuffer = new byte[CONSTANTS.BufferSize];
            StartReceiving();
        }

        public bool IsConnected()
        {
            return Tcp?.Connected == true;
        }

        public void Disconnect()
        {
            Log.LogWarning("Client disconnected");
            Tcp?.Close();
            Tcp = null;
            _tcpStream = null;
            _tcpReceiveBuffer = new byte[CONSTANTS.BufferSize];
        }

        public async Task SendAsync(byte[] bytes)
        {
            try
            {
                if (_tcpStream.CanWrite)
                {
                    await _tcpStream.WriteAsync(bytes, 0, bytes.Length);
                }
                else
                {
                    Log.LogWarning("Cannot write to the network stream.");
                }
            }
            catch (Exception e)
            {
                Log.LogWarning($"Error sending data: {e}");
                Disconnect();
            }
        }

        public async Task SendAsync(object obj)
        {
            if (_tcpStream.CanWrite)
            {
                byte[] bytes = Serialize(obj);
                await SendAsync(bytes);
            }
        }

        public void Send(object obj)
        {
            if (_tcpStream.CanWrite)
            {
                byte[] bytes = Serialize(obj);
                _tcpStream.Write(bytes, 0, bytes.Length);
            }
        }

        public void Send(byte[] bytes)
        {
            if (_tcpStream.CanWrite)
            {
                _tcpStream.Write(bytes, 0, bytes.Length);
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(data))
                {
                    return (T)_binaryFormatter.Deserialize(ms);
                }
            }
            catch (SerializationException e)
            {
                Log.LogWarning($"Error deserializing data: {e}");
                return default(T);
            }
        }

        public byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private async void StartReceiving()
        {
            try
            {
                while (IsConnected())
                {
                    int receivedLength = await _tcpStream.ReadAsync(_tcpReceiveBuffer, 0, CONSTANTS.BufferSize);
                    if (receivedLength > 0)
                    {
                        byte[] data = new byte[receivedLength];
                        Array.Copy(_tcpReceiveBuffer, data, receivedLength);
                        object msg = Deserialize<object>(data);
                        if (msg is Message)
                        {
                            NetworkHandler.Enqueue((Message)msg);
                        }
                        if (msg is Message[])
                        {
                            foreach (Message message in (Message[])msg)
                            {
                                NetworkHandler.Enqueue(message);
                            }
                        }
                    }
                    else
                    {
                        Log.LogWarning("Received 0 bytes - Disconnecting");
                        Disconnect();
                        break;
                    }
                }
            }
            catch (IOException e)
            {
                Log.LogWarning($"IOException while receiving TCP data: {e}");
                Disconnect();
                throw;
            }
            catch (Exception e)
            {
                Log.LogWarning($"Error receiving TCP data: {e}");
                Disconnect();
            }
        }

    }
}
