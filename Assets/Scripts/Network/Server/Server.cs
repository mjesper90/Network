using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
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
            AcceptClientsAsync();
        }

        // Polling for messages
        public void UpdateServer()
        {
            MatchMaking.UpdateMatches();
            CheckClientQueues();
        }

        // Shutdown and clear
        public void Shutdown()
        {
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
                Client client = new Client(tcp);
                Clients.Add(client);
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
                    UnityEngine.Debug.Log($"Client {client.NetworkHandler.Conn?.Username} has {client.NetworkHandler.MessageQueue.Count} messages");
                    while (client.NetworkHandler.MessageQueue.TryDequeue(out Message msg))
                    {
                        ProcessMessage(client, msg);
                    }
                }
                else
                {
                    UnityEngine.Debug.Log($"Client {client.NetworkHandler.Conn?.Username} is disconnected");
                    disconnectedClients.Add(client);
                }
            }
            Clients.RemoveAll(disconnectedClients.Contains);
        }

        private void ProcessMessage(Client client, Message msg)
        {
            if (msg.MsgType == MessageType.Login && client.NetworkHandler.Conn == null)
            {
                client.NetworkHandler.Conn = client.Deserialize<Connection>(msg.Data);
                UnityEngine.Debug.Log($"User {client.NetworkHandler.Conn.Username} logged in");
                client.Send(new Message(MessageType.LoginResponse, new byte[0], ""));
            }
            if (msg.MsgType == MessageType.JoinQueue && client.NetworkHandler.Conn != null)
            {
                client.NetworkHandler.InQueue = true;
                UnityEngine.Debug.Log($"User {client.NetworkHandler.Conn.Username} joined queue");
                MatchMaking.Join(client);
            }
        }
    }
}
