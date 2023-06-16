using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyShooter.DTOs;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using NetworkLib.GameServer;

namespace MyShooter
{
    public class NotQuakeMatch : Match
    {
        protected ConcurrentDictionary<string, List<Bullet>> _bullets = new ConcurrentDictionary<string, List<Bullet>>();
        protected ConcurrentDictionary<string, PositionAndYRotation> _playerPositions = new ConcurrentDictionary<string, PositionAndYRotation>();

        public override Message[] GetState()
        {
            List<Message> messages = new List<Message>();
            messages.AddRange(_playerPositions.Values);
            foreach (var kvp in _bullets)
            {
                messages.AddRange(kvp.Value);
            }
            _playerPositions.Clear();
            _bullets.Clear();

            return messages.ToArray();
        }

        protected override async Task ProcessMessage(Message msg, Client client)
        {
            //Server.Log.Log($"NotQuakeMatch processing message type {msg.GetType().Name} from {client.NetworkHandler.Auth.Username}");
            string username = client.NetworkHandler.Auth.Username;
            if (msg is PositionAndYRotation) // Position and rotation update
            {
                _playerPositions[username] = msg as PositionAndYRotation;
            }
            else if (msg is Bullet) // New bullet spawned
            {
                _bullets[username].Add(msg as Bullet);
            }
            else
            {
                //await base.ProcessMessage(msg, client);
            }
        }

        protected override async Task UpdateState()
        {
            await base.UpdateState();
            //Move bullets
            MoveBullets();
            //Bullet collision detection
            DetectBulletCollisions();
        }

        private void MoveBullets()
        {
            throw new NotImplementedException();
        }
        private void DetectBulletCollisions()
        {
            throw new NotImplementedException();
        }

    }
}