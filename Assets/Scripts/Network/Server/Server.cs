using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using NetworkLib.GameClient;
using NetworkLib.Common.Logger;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.Interfaces;

namespace NetworkLib.GameServer
{
    public class Server
    {
        #region Fields
        public int Port { get; }
        protected readonly TcpListener _tcpListener;
        protected readonly ConcurrentDictionary<Guid, Client> _clients = new ConcurrentDictionary<Guid, Client>();
        protected bool _isRunning = false;
        protected bool _isShuttingDown = false;

        // Matchmaking
        protected readonly MatchMaking _matchMaking;

        // Logger
        public static ILogNetwork Log;
        #endregion

        #region Constructors
        public Server(ILogNetwork log, int port, IMatch match)
        {
            Port = port;
            Log = log;
            _matchMaking = new MatchMaking(match);
            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            Log.Log($"Server started on port {Port}");
        }

        public Server(int port) : this(new DefaultLogger(), port, new Match()) { }
        #endregion

        #region Methods

        // Shutdown and clear
        public void Shutdown()
        {
            if (_isShuttingDown)
            {
                return; // Server is already shutting down
            }

            Log.Log("Server shutting down");
            _isShuttingDown = true;

            foreach (Client client in _clients.Values)
            {
                client.Disconnect();
            }

            _tcpListener.Stop();
            _clients.Clear();
        }

        // Start accepting clients
        public void StartAcceptingClients()
        {
            if (_isRunning)
            {
                Log.LogWarning("Server is already running");
                return; // Server is already running
            }
            Task.Run(async () =>
            {
                while (!_isShuttingDown)
                {
                    try
                    {
                        TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                        _ = HandleClientAsync(tcpClient);
                        Log.Log("Client connected");
                    }
                    catch (Exception e)
                    {
                        Log.LogWarning($"Exception: {e.Message}");
                        break;
                    }
                    await Task.Delay(1);
                }
            });
            Task.Run(async () =>
            {
                while (!_isShuttingDown)
                {
                    await CheckClientQueues();
                    await Task.Delay((int)(1000 * CONSTANTS.ServerSpeed));
                }
            });

            _isRunning = true;
        }

        // Handle connection
        protected async Task HandleClientAsync(TcpClient tcpClient)
        {
            Client client = new Client(Log, tcpClient);

            _clients.AddOrUpdate(client.Id, client, (key, oldClient) =>
            {
                oldClient.Disconnect();
                return client;
            });

            Log.Log("Client connected");

            await client.StartReceiving();

            _clients.TryRemove(client.Id, out _);
            Log.Log($"Client {client.NetworkHandler.Auth?.Username} disconnected");
        }

        protected async Task CheckClientQueues()
        {
            try
            {
                ConcurrentBag<Guid> disconnectedClientIds = new ConcurrentBag<Guid>();
                foreach (var clientPair in _clients)
                {
                    Client client = clientPair.Value;

                    if (client.IsConnected())
                    {
                        if (client.NetworkHandler.InGame)
                        {
                            continue;
                        }

                        int queueSize = client.NetworkHandler.GetQueueSize();
                        Log.Log($"Client {client.NetworkHandler.Auth?.Username} has {queueSize} messages");

                        foreach (Message msg in client.NetworkHandler.DequeueAll())
                        {
                            await ProcessMessage(client, msg);
                        }
                    }
                    else
                    {
                        Log.Log($"Client {client.NetworkHandler.Auth?.Username} is disconnected");
                        disconnectedClientIds.Add(client.Id);
                    }
                }

                foreach (Guid clientId in disconnectedClientIds)
                {
                    _clients.TryRemove(clientId, out _);
                }
            }
            catch (Exception e)
            {
                Log.LogWarning($"Exception: {e.Message}");
            }
        }

        protected async Task ProcessMessage(Client client, Message msg)
        {
            if (msg.MsgType == MessageType.Login && client.NetworkHandler.Auth == null)
            {
                client.NetworkHandler.Auth = client.Deserialize<Authentication>(msg.Data);
                Log.Log($"User {client.NetworkHandler.Auth.Username} logged in");
                _ = client.SendAsync(new Message(MessageType.LoginResponse));
            }
            if (msg.MsgType == MessageType.JoinQueue && client.NetworkHandler.Auth != null)
            {
                client.NetworkHandler.InQueue = true;
                Log.Log($"User {client.NetworkHandler.Auth.Username} joined queue");
                _ = _matchMaking.Join(client);
            }
        }
        #endregion
    }
}
