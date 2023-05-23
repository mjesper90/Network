using Network.Common;

namespace Network.ServerSide;

public class ServerPacket : Packet
{
    public uint ClientID;

    public ServerPacket(byte[] data, bool safePacket, int size, uint clientID) : base(data, size, safePacket)
    {
        ClientID = clientID;
    }

    public ServerPacket(Packet packet, uint clientID)
    {
        this.Data = packet.Data;
        this.Size = packet.Size;
        this.SafePacket = packet.SafePacket;
        ClientID = clientID;
    }
}
