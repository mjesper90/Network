using System.Threading.Tasks;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;

namespace NetworkLib.Common.Interfaces
{
    public interface IMatch
    {
        Task AddPlayer(Client client);
        Task Broadcast(Message msg);
        Task Broadcast(Message[] msgs);
        Task BroadcastExcept(Message msg, string username);
        Client[] GetClients();
        Message[] GetState();
        Task RemovePlayer(Client client);
        void Shutdown();
        void StartUpdateLoop();
        void StopUpdateLoop();
    }
}