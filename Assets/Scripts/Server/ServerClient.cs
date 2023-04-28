using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using DTOs;
using UnityEngine;

namespace GameServer
{
    public class ServerClient
    {
        public string Username;
        public TcpClient Socket; //Maybe UDP?

        private NetworkStream _stream;
        private byte[] _receiveBuffer;
        private readonly int _dataBuffer = CONSTANTS.BufferSize;
        private BinaryFormatter _br;
        public Network NetworkRef;

        public bool IsConnected()
        {
            return Socket != null;
        }

        public ServerClient(TcpClient socket)
        {
            _br = new BinaryFormatter();
            NetworkRef = new Network(this);
            Socket = socket;
            Socket.ReceiveBufferSize = _dataBuffer;
            Socket.SendBufferSize = _dataBuffer;
            _stream = Socket.GetStream();
            _receiveBuffer = new byte[_dataBuffer];
            _stream.BeginRead(_receiveBuffer, 0, _dataBuffer, RecieveCB, Socket);
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
                    NetworkRef.Recieve(obj);
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
                Debug.Log(e.Message);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            Socket.Close();
            Socket = null;
            _stream.Close();
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

        public void Update(ServerClient other)
        {
            if (IsConnected() && other.IsConnected())
            {
                Send(Serialize(other.NetworkRef.UserRef));
                //Handle projectiles
                foreach (Projectile projectile in other.NetworkRef.Projectiles)
                {
                    //UpdateCurrentPosition(projectile);
                    Send(Serialize(projectile));
                }
            }
        }

        private Position UpdateCurrentPosition(Projectile p)
        {
            throw new NotImplementedException();
        }
    }
}