

namespace UDP_Networker.Common;

public class Packet
{
    public uint ClientID;
    public uint PacketID;
    public byte[] Data;

    public Packet(uint clientID, uint packetID, byte[] data)
    {
        ClientID = clientID;
        PacketID = packetID;
        Data = data;
    }
}
