using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using DTOs;
using UnityEngine;

namespace GameClient
{
    public class Client
    {
        private const int _dataBufferSize = 4096;
        private readonly TcpClient _socket;
        private NetworkStream _stream;
        private readonly byte[] _recieveBuffer;
        private BinaryFormatter _br;
        private Network _network;

        public Client(string ip, int port)
        {
            _br = new BinaryFormatter();
            _socket = new TcpClient
            {
                ReceiveBufferSize = _dataBufferSize,
                SendBufferSize = _dataBufferSize
            };
            _recieveBuffer = new byte[_dataBufferSize];
            Debug.Log("Connecting to : " + IPAddress.Parse(ip));
            _socket.BeginConnect(IPAddress.Parse(ip), port, ConnectCB, null);
            _network = new Network();
        }

        public Network GetNetwork()
        {
            return _network;
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

            _stream.BeginRead(_recieveBuffer, 0, _dataBufferSize, RecieveCB, null);
        }

        private void RecieveCB(IAsyncResult ar)
        {
            try
            {
                int _recievedLength = _stream.EndRead(ar);
                if (_recievedLength > 0)
                {
                    byte[] data = new byte[_recievedLength];
                    Array.Copy(_recieveBuffer, data, _recievedLength);
                    object obj = Deserialize(data);
                    _network.Recieve(obj);
                    _stream.BeginRead(_recieveBuffer, 0, _dataBufferSize, RecieveCB, null);
                }
                else
                {
                    Disconnect();
                }
            }
            catch (Exception e)
            {
                Disconnect();
                Debug.Log("Error recieved TCP Data " + e.ToString());
            }
        }

        private void Disconnect()
        {
            Debug.Log("Disconnected");
            _stream.Close();
            _socket.Close();
        }
    }
}