using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using NetworkLib.Common;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.Common.Interfaces;
using NetworkLib.Common.Logger;
using NetworkLib.GameClient;

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
        protected readonly MatchMaking _mm;

        // Message Factory
        protected readonly MessageFactory _mf;

        // Logger
        public static ILogNetwork Log;
        #endregion

        #region Constructors
        public Server(ILogNetwork log, int port, IMatch match)
        {
            Port = port;
            Log = log;
            _mf = new MessageFactory(log, new BinaryFormatter());
            _mm = new MatchMaking(match);
            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            Log.Log($"Server started on port {Port}");
        }

        public Server(int port) : this(new DefaultLogger(), port, new Match()) { }
        #endregion

        #region Methods

        public Client[] GetClients()
        {
            return _clients.Values.ToArray();
        }

        // Shutdown and clear
        public void Shutdown()
        {
            if (_isShuttingDown)
            {
                return; // Server is already shutting down
            }

            Log.Log("Server shutting down");
            _isShuttingDown = true;
            Task.Delay(1000).Wait();
            _mm.Shutdown();

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
                    }
                    catch (Exception e)
                    {
                        Log.LogWarning($"Exception: {e.Message}");
                        break;
                    }
                    await Task.Delay(1); // Prevents CPU from going to 100%
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
            Client client = new Client(Log, tcpClient, _mf);

            _clients.AddOrUpdate(client.Id, client, (key, oldClient) =>
            {
                oldClient.Disconnect();
                return client;
            });

            Log.Log("Client connected");

            await client.StartReceiving();
            await _mm.Matches.ElementAt(0).RemovePlayer(client);
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
                        if (client.NetworkHandler.InGame) //Match is handling queue in game
                        {
                            continue;
                        }

                        int queueSize = client.NetworkHandler.GetQueueSize();
                        Log.Log($"Client {client.NetworkHandler.Auth?.Username} has {queueSize} messages");

                        foreach (Message msg in client.NetworkHandler.DequeueAll())
                        {
                            await HandleLoginAndQueue(client, msg);
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
                    _mm.Leave(clientId);
                    _clients.TryRemove(clientId, out _);
                }
            }
            catch (Exception e)
            {
                Log.LogWarning($"Exception: {e.Message}");
            }
        }

        protected async Task HandleLoginAndQueue(Client client, Message msg)
        {
            if (msg is Authentication && client.NetworkHandler.Auth == null)
            {
                client.NetworkHandler.Auth = msg as Authentication;
                //Check login or just assume it's correct?
                Log.Log($"User {client.NetworkHandler.Auth.Username} logged in");
                _ = client.SendAsync(new LoginResponse(true, "Login successful"));
            }
            else if (msg is QueueMessage && client.NetworkHandler.Auth != null)
            {
                client.NetworkHandler.InQueue = true;
                Log.Log($"User {client.NetworkHandler.Auth.Username} joined queue");
                _mm.Join(client);
            }
            else
            {
                Log.LogWarning($"Unknown message type: {msg.GetType()}");
            }
        }
        #endregion
    }
}
