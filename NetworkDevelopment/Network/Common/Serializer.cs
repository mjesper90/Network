using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Network.Common;

public class Serializer
{
    private static BinaryFormatter _binaryFormatter = new BinaryFormatter();

    public static byte[] GetData<T>(T obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            _binaryFormatter.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T DeSerialize<T>(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            return (T)_binaryFormatter.Deserialize(ms);
        }
    }
}
