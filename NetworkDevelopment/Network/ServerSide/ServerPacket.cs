using Network.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Networker.Server;

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
