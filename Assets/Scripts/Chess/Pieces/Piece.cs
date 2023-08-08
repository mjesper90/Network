using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour, Moveable
    {
        public abstract List<Tile> PossibleMoves();
        public abstract void MoveTo(Tile tile);
    }
}
