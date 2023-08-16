using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NetworkLib.Common.DTOs;

namespace NetworkLib.GameClient
{
    public class Network
    {
        public Authentication Auth = null;
        public bool InGame = false;
        public bool InQueue = false;

        private ConcurrentQueue<Message> _msgQueue = new ConcurrentQueue<Message>();

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

        public void Enqueue(Message msg)
        {
            _msgQueue.Enqueue(msg);
        }

        public int GetQueueSize()
        {
            return _msgQueue.Count;
        }

        public bool TryDequeue(out Message msg)
        {
            _msgQueue.TryDequeue(out msg);
            return msg != null;
        }
    }
}