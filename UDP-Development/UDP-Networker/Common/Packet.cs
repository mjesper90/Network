

namespace UDP_Networker.Common;

public class Packet
{
    public int ClientID;
    public int PacketID;
    public byte[] Data;

    public Packet(int clientID, int packetID, byte[] data)
    {
        ClientID = clientID;
        PacketID = packetID;
        Data = data;
    }
}
