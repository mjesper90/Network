using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using DTOs;

namespace GameServer
{
    public class ServerClient
    {
        public TcpClient Socket;
        public Network NetworkHandler;

        private NetworkStream _stream;
        private byte[] _receiveBuffer;
        private readonly int _dataBuffer = CONSTANTS.BufferSize;
        private BinaryFormatter _br;

        public ServerClient(TcpClient socket)
        {
            _br = new BinaryFormatter();
            NetworkHandler = new Network(this);
            Socket = socket;
            Socket.ReceiveBufferSize = _dataBuffer;
            Socket.SendBufferSize = _dataBuffer;
            _stream = Socket.GetStream();
            _receiveBuffer = new byte[_dataBuffer];
            _stream.BeginRead(_receiveBuffer, 0, _dataBuffer, RecieveCB, null);
        }

        public bool IsConnected()
        {
            return Socket != null;
        }

        private void RecieveCB(IAsyncResult ar)
        {
            try
            {
                int _recievedLength = _stream.EndRead(ar);
                if (_recievedLength > 0)
                {
                    byte[] data = new byte[_recievedLength];
                    Array.Copy(_receiveBuffer, data, _recievedLength);
                    object obj = Deserialize(data);
                    NetworkHandler.Recieve(obj);
                    _stream.BeginRead(_receiveBuffer, 0, _dataBuffer, RecieveCB, null);
                }
                else
                {
                    Disconnect();
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error recieving TCP data: {e}");
                Disconnect();
            }
        }

        public void Disconnect()
        {
            Socket.Close();
            Socket = null;
            _stream = null;
            _receiveBuffer = new byte[_dataBuffer];
        }

        public void Send(byte[] bytes)
        {
            if (_stream.CanWrite)
            {
                _stream.Write(bytes, 0, bytes.Length);
            }
        }

        public object Deserialize(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return _br.Deserialize(ms);
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
    }
}