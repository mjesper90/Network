using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public class Knight : Piece
    {
        public override List<Tile> PossibleMoves()
        {
            List<Tile> moves = new List<Tile>();
            if (CurrentTile != null)
            {
                Tile[,] tiles = CurrentTile.Board.Tiles;
                Tuple<int, int> tileIndex = CurrentTile.GetCoordinates();

                // Knight's movement offsets in all possible directions
                int[] rowOffsets = { 1, 1, -1, -1, 2, 2, -2, -2 };
                int[] colOffsets = { 2, -2, 2, -2, 1, -1, 1, -1 };

                for (int i = 0; i < 8; i++)
                {
                    int newRow = tileIndex.Item1 + rowOffsets[i];
                    int newCol = tileIndex.Item2 + colOffsets[i];

                    // Check if the new position is within the board boundaries
                    if (newRow >= 0 && newRow < tiles.GetLength(0) && newCol >= 0 && newCol < tiles.GetLength(1))
                    {
                        Tile newTile = tiles[newRow, newCol];

                        // Check if the destination tile is empty or contains an opponent's piece
                        if (newTile.CurrentPiece == null || newTile.CurrentPiece.Owner != Owner)
                        {
                            moves.Add(newTile);
                        }
                    }
                }
            }
            return moves;
        }
    }
}
