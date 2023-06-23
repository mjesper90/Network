using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skak.Pieces
{
    public class Piece : MonoBehaviour
    {
        public virtual List<int[]> PossibleMoves()
        {
            List<int[]> moves = new List<int[]>();
            // check every tile here for possible moves, set this up so it can only move forward or attack diagonally
            return moves;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Setup all the stuff for that any chess piece would need
        }

        // Update is called once per frame
        void Update()
        {
            // Check for collisions and such
        }

        bool Move()
        {
            if (ValidMove())
            {
                return true; // Shows success in moving the piece
            }
            else
            {
                return false; // Gives an error, not letting the user initiate such a move
            }
        }

        bool ValidMove()
        {
            // Check for the piece type initiating script, where to move and so on

            return true;
        }
    }
}
