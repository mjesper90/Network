using Chess.Pieces;

namespace Chess
{
    public class BoardLog
    {
        public Tile MoveFromTile, MoveToTile;

        public BoardLog(Tile moveFromTile, Tile moveToTile)
        {
            MoveFromTile = moveFromTile;
            MoveToTile = moveToTile;
        }

        public override string ToString()
        {
            return $"from {MoveFromTile} to {MoveToTile}";
        }
    }
}