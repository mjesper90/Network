using Network.ClientSide;
using Network.Common;
using System.Net;

namespace ExampleGameTicTacToe;

public class cBatchHeader
{
    public DateTime TimeStamp;
}

public class cBatchData
{
    // Square that is being hoverd over
    public int SelectedSquare;

    // [0, 9] or -1 for no move yet
    public int Move;
}

public class cBatch : Batch<cBatchHeader, cBatchData>
{
    public cBatch(cBatchHeader header, cBatchData data) : base(header, data)
    {
    }
}

internal class TicTacToeClient
{
    // Networking
    private Client _client = new Client("Jack");

    private TicTacToe _board = new TicTacToe();
    private int _ourPlayerID;

    // Game logic
    private int _selectedSquare;

    private bool SendUpdate(int move = -1)
    {
        cBatchHeader header = new cBatchHeader();
        header.TimeStamp = DateTime.Now;

        cBatchData data = new cBatchData();
        data.SelectedSquare = _selectedSquare;
        data.Move = move;

        if (!_client.IsConnecting)
            return false;
        _client.SendData(new cBatch(header, data));
        return true;
    }

    public void Start(IPEndPoint ip)
    {
        _client.Connect(ip);

        if (!_client.IsConnected)
        {
            Console.WriteLine("Could no connect to server");
            return;
        }

        int i = 0;
        while (true)
        {
            Thread.Sleep(10);
            _client.SendData(i++, false);
        }
    }
}
