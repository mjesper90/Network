using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using DTOs;

namespace GameServer
{
    public class Server
    {
        public int Port { get; private set; }
        public TcpListener TCPListener;
        public List<ServerClient> Clients = new List<ServerClient>();

        public Server(int port)
        {
            Port = port;
            TCPListener = new TcpListener(IPAddress.Any, Port);
            TCPListener.Start();
            TCPListener.BeginAcceptTcpClient(new AsyncCallback(TCPAcceptCB), null);
        }

        //Accept new connection
        private void TCPAcceptCB(IAsyncResult ar)
        {
            TcpClient client = TCPListener.EndAcceptTcpClient(ar);
            TCPListener.BeginAcceptTcpClient(new AsyncCallback(TCPAcceptCB), null);
            ServerClient serverClient = new ServerClient(client);
            Clients.Add(serverClient);
        }

        //Shutdown and clear
        public void Shutdown()
        {
            foreach (ServerClient client in Clients)
            {
                client.Disconnect();
            }
            TCPListener.Stop();
        }

        public void Update()
        {
            List<ServerClient> disconnectedClients = new List<ServerClient>();
            Batch batch = new Batch(new Projectile[0], new User[0]);
            foreach (ServerClient client in Clients)
            {
                if (client.IsConnected())
                {
                    batch.Append(client.NetworkHandler.GetBatch());
                }
                else
                {
                    disconnectedClients.Add(client);
                }
            }
            Clients.RemoveAll(disconnectedClients.Contains);

            if (batch.Users.Length <= 0) return;

            foreach (ServerClient client in Clients)
            {
                client.NetworkHandler.SendBatch(batch);
            }
        }
    }
}