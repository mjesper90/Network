using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameClient
{
    public class Client
    {
        public Network NetworkHandler;

        private int _dataBufferSize = CONSTANTS.BufferSize;
        private readonly TcpClient _socket;
        private NetworkStream _stream;
        private readonly byte[] _recieveBuffer;
        private BinaryFormatter _br;

        public Client(string ip, int port)
        {
            _br = new BinaryFormatter();
            _socket = new TcpClient
            {
                ReceiveBufferSize = _dataBufferSize,
                SendBufferSize = _dataBufferSize
            };
            _recieveBuffer = new byte[_dataBufferSize];
            _socket.BeginConnect(IPAddress.Parse(ip), port, ConnectCB, null);
            NetworkHandler = new Network();
        }

        public void Send(byte[] bytes)
        {
            if (_stream.CanWrite)
            {
                _stream.Write(bytes, 0, bytes.Length);
            }
        }

        public void Send(object obj)
        {
            byte[] bytes = Serialize(obj);
            Send(bytes);
        }

        private object Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return _br.Deserialize(ms);
            }
        }

        private byte[] Serialize(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                _br.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private void ConnectCB(IAsyncResult ar)
        {
            _socket.EndConnect(ar);
            if (!_socket.Connected)
            {
                return;
            }
            _stream = _socket.GetStream();

            _stream.BeginRead(_recieveBuffer, 0, _dataBufferSize, ReceiveCB, null);
        }

        private void ReceiveCB(IAsyncResult ar)
        {
            try
            {
                int _recievedLength = _stream.EndRead(ar);
                if (_recievedLength > 0)
                {
                    byte[] data = new byte[_recievedLength];
                    Array.Copy(_recieveBuffer, data, _recievedLength);
                    object obj = Deserialize(data);
                    NetworkHandler.Receive(obj);
                    _stream.BeginRead(_recieveBuffer, 0, _dataBufferSize, ReceiveCB, null);
                }
                else
                {
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error recieving TCP data: {e}");
                Disconnect();
            }
        }

        private void Disconnect()
        {
            _stream.Close();
            _socket.Close();

            Console.WriteLine("Disconnected from server.");
        }
    }
}