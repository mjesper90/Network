using System.Threading.Tasks;
using NetworkLib.GameClient;

namespace NetworkLib.Common.Interfaces
{
    public interface IMatchMaking
    {
        void Join(Client client);
    }
}