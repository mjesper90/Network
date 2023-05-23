

using Network;
using Network.Common;
using Network.ServerSide;

namespace ExampleGameTicTacToe;

public enum sDataType
{
    // Indicates that
    NumberAssign,
    BoardUpdate
}

public class sBatchHeader
{
    public DateTime TimeStamp;
    public sDataType DataType;
}

public class sBatchData
{
    // How to board is, because its from the server
    public TicTacToe Board;

    // The number asociated with the player reciving this
    public int ReciverPlayerID;
}

public class sBatch : Batch<sBatchHeader, sBatchData>
{
    public sBatch(sBatchHeader header, sBatchData data) : base(header, data)
    {
    }
}

internal class TicTacToeServer
{
    private Server _server = new Server();

    public void Start()
    {
        // Log server ip


        while (true)
        {
            //Logger.Log("Server has x players: " + _server.GetCurrentClientHandles().Length, LogWarningLevel.Debug);
            var packets = _server.Update();
            string str = "";
            foreach (var item in packets)
            {
                str += Serializer.GetObject<int>(item) + "\n";
            }
            Console.Write(str);
            Thread.Sleep(16);
        }
    }
}
