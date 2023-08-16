using System.Collections.Generic;

namespace Chess.Pieces
{
    public interface Moveable
    {
        List<Tile> PossibleMoves();
        void MoveTo(Tile tile);
    }
}