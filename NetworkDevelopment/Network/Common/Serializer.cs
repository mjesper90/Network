using System.Runtime.Serialization.Formatters.Binary;

namespace Network.Common;

public class Serializer
{
    private static BinaryFormatter _binaryFormatter = new BinaryFormatter();

    public static byte[] GetBytes<T>(T obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            _binaryFormatter.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T GetObject<T>(byte[] data, int startIndex = 0, int count = -1)
    {
        if (count == -1)
            count = data.Length - startIndex;
        using (MemoryStream ms = new MemoryStream(data, startIndex, count))
        {
            return (T)_binaryFormatter.Deserialize(ms);
        }
    }
}
