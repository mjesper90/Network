using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Networker.Common;

public class Serializer
{
    private static BinaryFormatter _binaryFormatter = new BinaryFormatter();

    public static byte[] GetData(object obj)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            _binaryFormatter.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static object DeSerialize(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            return _binaryFormatter.Deserialize(ms);
        }
    }
}
