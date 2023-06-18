using Network.Common;

namespace Network.ServerSide;

public class ServerPacket : Packet
{
    public Guid ClientID;

    public ServerPacket(byte[] data, bool safePacket, int size, Guid clientID) : base(data, size, safePacket)
    {
        ClientID = clientID;
    }

    public ServerPacket(Packet packet, Guid clientID)
    {
        this.Data = packet.Data;
        this.Size = packet.Size;
        this.SafePacket = packet.SafePacket;
        ClientID = clientID;
    }
}
