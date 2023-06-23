using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public class Queen : Piece
    {
        public override List<int[]> PossibleMoves()
        {
            List<int[]> moves = new List<int[]>();
            // check every tile here for possible moves, set this up so it can only move forward or attack diagonally
            return moves;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}