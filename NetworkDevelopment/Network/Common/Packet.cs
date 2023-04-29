

namespace Network.Common;

public class Packet
{
    public uint ClientID;

    /// <summary>
    /// Indicates if the data is send over TCP (True) or UDP (False)
    /// </summary>
    public bool SafePacket;
    public int Size;
    public byte[] Data;

    public Packet(byte[] data, bool safePacket, int size, uint clientID)
    {
        Data = data;
        SafePacket = safePacket;
        Size = size;
        ClientID = clientID;
    }
}
