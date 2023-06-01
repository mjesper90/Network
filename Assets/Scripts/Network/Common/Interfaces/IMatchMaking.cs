using System.Threading.Tasks;
using NetworkLib.GameClient;

namespace NetworkLib.Common.Interfaces
{
    public interface IMatchMaking
    {
        Task Join(Client client);
    }
}