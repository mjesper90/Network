using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NetworkLib.Common.DTOs;

namespace NetworkLib.GameClient
{
    public class Network
    {
        public Client Client;
        public Authentication Auth = null;
        public bool InGame = false;
        public bool InQueue = false;

        private ConcurrentQueue<Message> _msgQueue = new ConcurrentQueue<Message>();

        public Network(Client client)
        {
            Client = client;
        }

        public void Enqueue(Message msg)
        {
            _msgQueue.Enqueue(msg);
        }

        public bool TryDequeue(out Message msg)
        {
            _msgQueue.TryDequeue(out msg);
            return msg != null;
        }

        public int GetQueueSize()
        {
            return _msgQueue.Count;
        }

        public void ClearQueue()
        {
            _msgQueue = new ConcurrentQueue<Message>();
        }

        public List<Message> DequeueAll()
        {
            List<Message> messages = new List<Message>();
            while (TryDequeue(out Message msg))
            {
                messages.Add(msg);
            }
            return messages;
        }
    }
}