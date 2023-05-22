using System.Collections.Concurrent;
using DTOs;

namespace NetworkLib.GameClient
{
    public class Network
    {
        public Client Client;
        public Connection Conn = null;
        public bool InGame = false;
        public bool InQueue = false;

        public ConcurrentQueue<Message> MessageQueue = new ConcurrentQueue<Message>();

        public Network(Client client)
        {
            Client = client;
        }
    }
}