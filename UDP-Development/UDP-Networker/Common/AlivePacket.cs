

namespace Networker.Common;

/// <summary>
/// We use this once in a while to check if the connection is still there. If this dose not get send for a sertain while the connection may be lost.
/// </summary>
internal class AlivePacket : Packet
{
    // Random bytes to verify the the connection is still alive
    private static readonly byte[] _data = { 255, 255, 128, 128, 1, 1 };

    public AlivePacket(uint clientID, int packetID)
        : base(_data, true, packetID, clientID)
    {

    }

    public static bool ValidateAlivePacket(AlivePacket alivePacket)
    {
        if (alivePacket.Data.Length != _data.Length)
            return false;

        bool Match = true;
        for (int i = 0; i < _data.Length; i++)
            if (_data[i] != alivePacket.Data[i])
            {
                Match = false;
                break;
            }

        return Match;
    }
}
