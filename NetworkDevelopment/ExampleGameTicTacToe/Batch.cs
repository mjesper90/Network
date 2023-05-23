

namespace ExampleGameTicTacToe;

[Serializable]
internal class Batch
{
    // Header
    public DateTime SendAt;

    // Data
    public TicTacToe Board;
}
