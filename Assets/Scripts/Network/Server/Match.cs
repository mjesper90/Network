using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetworkLib.Common.Interfaces;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using System;

namespace NetworkLib.GameServer
{
    public class Match : IMatch
    {
        protected ConcurrentDictionary<string, Client> _clients { get; } = new ConcurrentDictionary<string, Client>();
        protected ConcurrentDictionary<string, Message> _lastMessages = new ConcurrentDictionary<string, Message>();

        private CancellationTokenSource _tokenSource;
        private Task _updateTask;

        public Client[] GetClients()
        {
            return _clients.Values.ToArray();
        }

        public async Task AddPlayer(Client client)
        {
            _clients.TryAdd(client.NetworkHandler.Auth.Username, client);

            // Notify other clients in the match about the new player
            IEnumerable<Task> tasks = _clients.Values
                .Where(c => c.NetworkHandler.Auth.Username != client.NetworkHandler.Auth.Username)
                .Select(c => c.SendAsync(new Message(MessageType.PlayerJoined, c.Serialize(client.NetworkHandler.Auth.Username))));

            await Task.WhenAll(tasks);
        }

        public async Task RemovePlayer(Client client)
        {
            _clients.TryRemove(client.NetworkHandler.Auth.Username, out _);

            // Notify other clients in the match about the removed player
            IEnumerable<Task> tasks = _clients.Values
                .Where(c => c.NetworkHandler.Auth.Username != client.NetworkHandler.Auth.Username)
                .Select(c => c.SendAsync(new Message(MessageType.PlayerLeft, c.Serialize(client.NetworkHandler.Auth.Username))));

            await Task.WhenAll(tasks);
        }

        public virtual Message[] GetState()
        {
            return _lastMessages.Values.ToArray();
        }

        public void StartUpdateLoop()
        {
            _tokenSource = new CancellationTokenSource();
            _updateTask = Task.Run(RunUpdateLoop);
        }

        public void StopUpdateLoop()
        {
            _tokenSource?.Cancel();
            _updateTask?.Wait(); // Wait for the update loop to complete
        }

        public virtual async Task UpdateState()
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

        protected async Task RunUpdateLoop()
        {
            while (!_tokenSource.Token.IsCancellationRequested)
            {
                await UpdateState();
                UpdateClients();
                await Task.Delay((int)(CONSTANTS.ServerSpeed * 1000)); // Delay between iterations
            }
        }

        private async void UpdateClients()
        {
            await Broadcast(GetState());
        }

        protected async Task ProcessMessage(Message msg, Client client)
        {
            await BroadcastExcept(msg, client.NetworkHandler.Auth.Username);
        }
    }
}