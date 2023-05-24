using System;
using System.Collections.Concurrent;
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
            //Client.Log.Log($"Enqueueing message {msg.MsgType}");
            _msgQueue.Enqueue(msg);
        }

        public bool TryDequeue(out Message msg)
        {
            _msgQueue.TryDequeue(out msg);
            //if (msg != null)
                //Client.Log.Log($"Dequeueing message {msg.MsgType}");
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
    }
}