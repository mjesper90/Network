

namespace Network.Common;

[Serializable]
internal class Batch<HeaderType, DataType>
{
    HeaderType Header;
    DataType Data;

    public Batch(HeaderType header, DataType data)
    {
        Header = header;
        Data = data;
    }
}