

using System.Drawing;
using System.Runtime.CompilerServices;

namespace Network.Common;

public class Packet
{
    /// <summary>
    /// Indicates if the data is send over TCP (True) or UDP (False). ( True by default )
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

    public Packet() { }

    public Packet(byte[] data)
    {
        Data = data;
        Size = data.Length;
        SafePacket = true;
    }

    public Packet(byte[] data, bool safePacket)
    {
        Data = data;
        Size = data.Length;
        SafePacket = safePacket;
    }

    public Packet(byte[] data, int size, bool safePacket)
    {
        Data = data;
        Size = size;
        SafePacket = safePacket;
    }

    public Packet ChangeSafty(bool safty)
    {
        SafePacket = safty;
        return this;
    }

    public static implicit operator Packet(byte[] data) => new Packet(data);
}
