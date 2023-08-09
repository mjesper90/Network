using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public List<Tile> LastCheckedMoves = new List<Tile>();

        public override List<Tile> PossibleMoves()
        {
            List<Tile> moves = new List<Tile>();
            if (CurrentTile != null)
            {
                Tile[,] tiles = CurrentTile.Board.Tiles;
                Tuple<int, int> tileIndex = CurrentTile.GetCoordinates();
                Player owner = Owner;

                //If white and hasn't moved
                if (owner == GameController.Instance.WhitePlayer)
                {
                    //Check if tile in front is empty
                    if (tiles[tileIndex.Item1 + 1, tileIndex.Item2].GetPiece() == null)
                    {
                        moves.Add(tiles[tileIndex.Item1 + 1, tileIndex.Item2]);
                        //Check if tile two in front is empty
                    }
                }

            }
            LastCheckedMoves = moves;
            return moves;
        }
    }
}