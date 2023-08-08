using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.Common.Interfaces;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public class Match : IMatch
    {
        protected ConcurrentDictionary<string, Client> _clients = new ConcurrentDictionary<string, Client>();
        protected Guid _id = Guid.NewGuid();
        protected ConcurrentDictionary<string, Message> _lastMessages = new ConcurrentDictionary<string, Message>();
        protected CancellationTokenSource _tokenSource;
        protected Task _updateClientsTask;
        protected Task _updateStateTask;

        public virtual async Task AddPlayer(Client client)
        {
            _clients.TryAdd(client.NetworkHandler.Auth.Username, client);
            Server.Log.Log("Match: Added player " + client.NetworkHandler.Auth.Username);

            // Notify other clients in the match about the new player
            IEnumerable<Task> tasks = _clients.Values
                .Where(c => c.NetworkHandler.Auth.Username != client.NetworkHandler.Auth.Username)
                .Select(c => c.SendAsync(new PlayerJoined(client.NetworkHandler.Auth.Username, _id.ToString())));

            _ = Task.WhenAll(tasks);
            await client.SendAsync(new MatchMessage(_id.ToString(), _clients.Values.Select(c => c.NetworkHandler.Auth.Username).ToArray()));
        }

        public async Task Broadcast(Message msg)
        {
            await Task.WhenAll(_clients.Values.Select(c => c.SendAsync(msg)));
        }

        public async Task Broadcast(Message[] msgs)
        {
            await Task.WhenAll(_clients.Values.Select(c => c.SendAsync(msgs)));
        }

        public async Task BroadcastExcept(Message msg, string username)
        {
            await Task.WhenAll(_clients.Values
                .Where(c => c.NetworkHandler.Auth.Username != username)
                .Select(c => c.SendAsync(msg)));
        }

        public Client[] GetClients()
        {
            return _clients.Values.ToArray();
        }

        public virtual Message[] GetState()
        {
            return _lastMessages.Values.ToArray();
        }

        public virtual async Task RemovePlayer(Client client)
        {
            Server.Log.Log("Match: Removed player " + client.NetworkHandler.Auth.Username);
            _clients.TryRemove(client.NetworkHandler.Auth.Username, out _);
            //Remove player from last messages
            _lastMessages.TryRemove(client.NetworkHandler.Auth.Username, out _);

            // Notify other clients in the match about the removed player
            IEnumerable<Task> tasks = _clients.Values
                .Where(c => c.NetworkHandler.Auth.Username != client.NetworkHandler.Auth.Username)
                .Select(c => c.SendAsync(new PlayerLeft(client.NetworkHandler.Auth.Username, _id.ToString())));

            await Task.WhenAll(tasks);
        }

        public void StartUpdateLoop()
        {
            _tokenSource = new CancellationTokenSource();
            _updateStateTask = Task.Run(UpdateStateLoop);
            _updateClientsTask = Task.Run(UpdateClientsLoop);
        }

        public void StopUpdateLoop()
        {
            _tokenSource?.Cancel();
            _updateStateTask?.Wait();
            _updateClientsTask?.Wait();
        }

        protected virtual async Task ProcessMessage(Message msg, Client client)
        {
            _lastMessages[client.NetworkHandler.Auth.Username] = msg;
            await BroadcastExcept(msg, client.NetworkHandler.Auth.Username);
        }

        protected async void UpdateClients()
        {
            Message[] msgs = GetState();
            if (msgs.Length == 0 || _clients.Count == 0)
                return;
            await Broadcast(msgs);
        }

        protected async Task UpdateClientsLoop()
        {
            while (!_tokenSource.Token.IsCancellationRequested)
            {
                UpdateClients();
                await Task.Delay((int)(CONSTANTS.ServerSpeed * 1000)); // Delay between iterations
            }
        }

        protected virtual async Task UpdateState()
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(_clients.Values, c =>
                {
                    if (c.NetworkHandler.Auth == null) //Player not logged in?
                    {
                        Server.Log.LogWarning("Match: Player not logged in");
                        return;
                    }

                    while (c.NetworkHandler.TryDequeue(out Message msg))
                    {
                        _ = ProcessMessage(msg, c);
                    }
                });
            });
        }

        protected async Task UpdateStateLoop()
        {
            while (!_tokenSource.Token.IsCancellationRequested)
            {
                await UpdateState();
                await Task.Delay(1);
            }
        }
    }
}