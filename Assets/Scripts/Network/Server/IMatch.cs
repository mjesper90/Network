using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;

namespace NetworkLib.GameServer
{
    public interface IMatch
    {
        Client[] GetClients();
        void AddPlayer(Client client);
        void RemovePlayer(Client client);
        Message[] GetState();
        void UpdateState();
        void Broadcast(Message msg);
    }
}