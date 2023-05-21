using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using DTOs;
using GameClient;

namespace GameServer
{
    public class Server
    {
        public int Port { get; private set; }
        public TcpListener TCPListener;
        public List<Client> Clients = new List<Client>();
        public MatchMaking MatchMaking = new MatchMaking();

        public Server(int port)
        {
            Port = port;
            TCPListener = new TcpListener(IPAddress.Any, Port);
            TCPListener.Start();
            TCPListener.BeginAcceptTcpClient(new AsyncCallback(TCPAcceptCallback), null);
        }

        //Accept new connection
        private void TCPAcceptCallback(IAsyncResult ar)
        {
            TcpClient tcp = TCPListener.EndAcceptTcpClient(ar);
            TCPListener.BeginAcceptTcpClient(new AsyncCallback(TCPAcceptCallback), null);
            Client client = new Client(tcp);
            Clients.Add(client);
        }

        //polling for messages
        public void UpdateServer()
        {
            MatchMaking.UpdateMatches();
            CheckClientQueues();
        }

        private void CheckClientQueues()
        {
            List<Client> disconnectedClients = new List<Client>();
            foreach (Client client in Clients)
            {
                if (client.IsConnected())
                {
                    if (client.NetworkHandler.InGame)
                    {
                        continue;
                    }
                    UnityEngine.Debug.Log($"Client {client.NetworkHandler.User?.Username} has {client.NetworkHandler.MessageQueue.Count} messages");
                    while (client.NetworkHandler.MessageQueue.Count > 0)
                    {
                        if (client.NetworkHandler.MessageQueue.TryDequeue(out Message msg))
                        {
                            if (msg.MsgType == MessageType.Login && client.NetworkHandler.User == null)
                            {
                                client.NetworkHandler.User = client.Deserialize<User>(msg.Data);
                                UnityEngine.Debug.Log($"User {client.NetworkHandler.User.Username} logged in");
                                client.Send(new Message(MessageType.LoginResponse, new byte[0], ""));
                            }
                            if (msg.MsgType == MessageType.JoinQueue && client.NetworkHandler.User != null)
                            {
                                client.NetworkHandler.InQueue = true;
                                UnityEngine.Debug.Log($"User {client.NetworkHandler.User.Username} joined queue");
                                MatchMaking.Join(client);
                            }
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.Log($"Client {client.NetworkHandler.User?.Username} is disconnected");
                    disconnectedClients.Add(client);
                }
            }
            Clients.RemoveAll(disconnectedClients.Contains);
        }

        //Shutdown and clear
        public void Shutdown()
        {
            foreach (Client client in Clients)
            {
                client.Disconnect();
            }
            TCPListener.Stop();
        }
    }
}