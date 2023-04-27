using UnityEngine;
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
            Debug.Log("Server starting on port " + Port + "...");
            TCPListener = new TcpListener(IPAddress.Any, Port);
            TCPListener.Start();
            TCPListener.BeginAcceptTcpClient(new AsyncCallback(TCPAcceptCB), null);
        }

        //Accept new connection
        private void TCPAcceptCB(IAsyncResult ar)
        {
            TcpClient client = TCPListener.EndAcceptTcpClient(ar);
            TCPListener.BeginAcceptTcpClient(new AsyncCallback(TCPAcceptCB), null);
            Debug.Log("Incoming connection from " + client.Client.RemoteEndPoint + "...");
            ServerClient serverClient = new ServerClient(client);
            Clients.Add(serverClient);
        }

        //Shutdown and clear
        public void Shutdown()
        {
            Debug.Log("Server shutting down...");
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
                    batch.Append(client.NetworkRef.GetBatch());
                }
                else
                {
                    disconnectedClients.Add(client);
                }
            }
            Clients.RemoveAll(disconnectedClients.Contains);

            if (batch.Users.Length <= 0) return;
            //Debug.Log("Sending batch of " + batch.Users.Length + "users, to clients..." + batch.Users[0].Username);
            foreach (ServerClient client in Clients)
            {
                client.NetworkRef.SendBatch(batch);
            }
        }
    }
}