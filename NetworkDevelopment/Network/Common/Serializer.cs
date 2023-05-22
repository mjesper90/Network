using System.Runtime.Serialization.Formatters.Binary;

namespace Network.Common;

public class Serializer
{
    private static BinaryFormatter _binaryFormatter = new BinaryFormatter();

    public static Packet GetPacket<T>(T obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            _binaryFormatter.Serialize(ms, obj);
            return (Packet)ms.ToArray();
        }
    }

    public static T GetObject<T>(Packet packet)
    {
        using (MemoryStream ms = new MemoryStream(packet.Data, 0, packet.Size))
        {
            return (T)_binaryFormatter.Deserialize(ms);
        }
    }
}
