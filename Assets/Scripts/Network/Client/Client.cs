using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using NetworkLib;

namespace NetworkLib.GameClient
{
    public class Client
    {
        public TcpClient Tcp;
        public Network NetworkHandler;

        private NetworkStream _tcpStream;
        private byte[] _TcpReceiveBuffer;
        private BinaryFormatter _br;

        public Client(TcpClient socket)
        {
            _br = new BinaryFormatter();
            NetworkHandler = new Network(this);
            Tcp = socket;
            Tcp.ReceiveBufferSize = CONSTANTS.BufferSize;
            Tcp.SendBufferSize = CONSTANTS.BufferSize;
            _tcpStream = Tcp.GetStream();
            _TcpReceiveBuffer = new byte[CONSTANTS.BufferSize];
            _tcpStream.BeginRead(_TcpReceiveBuffer, 0, CONSTANTS.BufferSize, RecieveCallback, null);
        }

        public bool IsConnected()
        {
            return Tcp?.Connected == true;
        }

        public void Disconnect()
        {
            Tcp?.Close();
            Tcp = null;
            _tcpStream = null;
            _TcpReceiveBuffer = new byte[CONSTANTS.BufferSize];
        }

        public async Task SendAsync(byte[] bytes)
        {
            if (_tcpStream.CanWrite)
            {
                await _tcpStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }

        public async Task SendAsync(object obj)
        {
            if (_tcpStream.CanWrite)
            {
                byte[] bytes = Serialize(obj);
                await _tcpStream.WriteAsync(bytes, 0, bytes.Length);
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
            using (MemoryStream ms = new MemoryStream(data))
            {
                return (T)_br.Deserialize(ms);
            }
        }

        public byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _br.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private void RecieveCallback(IAsyncResult ar)
        {
            try
            {
                int _recievedLength = _tcpStream.EndRead(ar);
                if (_recievedLength > 0)
                {
                    byte[] data = new byte[_recievedLength];
                    Array.Copy(_TcpReceiveBuffer, data, _recievedLength);
                    object msg = Deserialize<object>(data);
                    if (msg is Message)
                    {
                        NetworkHandler.MessageQueue.Enqueue((Message)msg);
                    }
                    if (msg is Message[])
                    {
                        foreach (Message message in (Message[])msg)
                        {
                            NetworkHandler.MessageQueue.Enqueue(message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Client disconnected");
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error recieving TCP data: {e}");
                Disconnect();
            }
            finally
            {
                _tcpStream.BeginRead(_TcpReceiveBuffer, 0, CONSTANTS.BufferSize, RecieveCallback, null);
            }
        }

    }
}