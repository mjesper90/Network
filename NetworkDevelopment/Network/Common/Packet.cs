

namespace Network.Common;

public class Packet
{
    public uint ClientID;

    /// <summary>
    /// Indicates if the data is send over TCP (True) or UDP (False)
    /// </summary>
    public bool SafePacket;

    /// <summary>
    /// Amout of bytes read
    /// </summary>
    public int Size;

    /// <summary>
    /// Only valid data up to Size
    /// </summary>
    public byte[] Data;

    public Packet(byte[] data, bool safePacket, int size, uint clientID)
    {
        Data = data;
        SafePacket = safePacket;
        Size = size;
        ClientID = clientID;
    }
}
