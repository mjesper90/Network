using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NetworkLib.GameClient;
using NetworkLib.Common.Logger;
using NetworkLib.Common.DTOs;

namespace NetworkLib.GameServer
{
    public class Server
    {
        public int Port { get; private set; }
        public TcpListener TCPListener;
        public List<Client> Clients = new List<Client>();

        // Matchmaking
        public MatchMaking MatchMaking = new MatchMaking();

        // Logger
        public static ILogNetwork Log;

        public Server(ILogNetwork log, int port)
        {
            Port = port;
            Log = log;
            TCPListener = new TcpListener(IPAddress.Any, Port);
            TCPListener.Start();
            AcceptClientsAsync();
        }

        public Server(int port)
        {
            Port = port;
            Log = new DefaultLogger();
            TCPListener = new TcpListener(IPAddress.Any, Port);
            TCPListener.Start();
            AcceptClientsAsync();
        }

        // Polling for message queues
        public void UpdateServer()
        {
            MatchMaking.UpdateMatches();
            CheckClientQueues();
        }

        // Shutdown and clear
        public void Shutdown()
        {
            Log.Log("Server shutting down");
            foreach (Client client in Clients)
            {
                client.Disconnect();
            }
            TCPListener.Stop();
        }

        // Accept new connections asynchronously
        private async void AcceptClientsAsync()
        {
            while (true)
            {
                TcpClient tcp = await TCPListener.AcceptTcpClientAsync();
                Client client = new Client(Log, tcp);
                Clients.Add(client);
                Log.Log($"Client {client.NetworkHandler.Auth?.Username} connected");
            }
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
                    Log.Log($"Client {client.NetworkHandler.Auth?.Username} has {client.NetworkHandler.GetQueueSize()} messages");
                    while (client.NetworkHandler.TryDequeue(out Message msg))
                    {
                        ProcessMessage(client, msg);
                    }
                }
                else
                {
                    Log.Log($"Client {client.NetworkHandler.Auth?.Username} is disconnected");
                    disconnectedClients.Add(client);
                }
            }
            Clients.RemoveAll(disconnectedClients.Contains);
        }

        private void ProcessMessage(Client client, Message msg)
        {
            if (msg.MsgType == MessageType.Login && client.NetworkHandler.Auth == null)
            {
                client.NetworkHandler.Auth = client.Deserialize<Authentication>(msg.Data);
                Log.Log($"User {client.NetworkHandler.Auth.Username} logged in");
                client.Send(new Message(MessageType.LoginResponse));
            }
            if (msg.MsgType == MessageType.JoinQueue && client.NetworkHandler.Auth != null)
            {
                client.NetworkHandler.InQueue = true;
                Log.Log($"User {client.NetworkHandler.Auth.Username} joined queue");
                MatchMaking.Join(client);
            }
        }
    }
}
