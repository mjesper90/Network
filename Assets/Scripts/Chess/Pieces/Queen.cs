using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public class Queen : Piece
    {
        public override List<Tile> PossibleMoves()
        {
            List<Tile> moves = new List<Tile>();
            


            return moves;
        }

        public override void MoveTo(Tile tile)
        {
            throw new System.NotImplementedException();
        }
    }
}