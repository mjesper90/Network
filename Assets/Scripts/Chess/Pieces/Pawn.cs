using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public class Pawn : Piece
    {
        public override List<Tile> PossibleMoves()
        {
            List<Tile> moves = new List<Tile>();
            if (CurrentTile != null)
            {
                Tile[,] tiles = CurrentTile.Board.Tiles;
                Tuple<int, int> tileIndex = CurrentTile.GetCoordinates();

                int forwardOffset = (Owner == CurrentTile.Board.WhitePlayer) ? 1 : -1; // Pawns move forward based on their color

                // Forward movement
                int newRow = tileIndex.Item1 + forwardOffset;
                int newCol = tileIndex.Item2;

                if (newRow >= 0 && newRow < tiles.GetLength(0))
                {
                    Tile newTile = tiles[newRow, newCol];

                    if (newTile.CurrentPiece == null)
                    {
                        moves.Add(newTile);

                        // Check for initial double move
                        if (IsStartingPosition(tileIndex.Item1) && tiles[newRow + forwardOffset, newCol].CurrentPiece == null)
                        {
                            moves.Add(tiles[newRow + forwardOffset, newCol]);
                        }
                    }
                }

                // Capture diagonally
                int[] captureOffsets = { -1, 1 };
                foreach (int offset in captureOffsets)
                {
                    newCol = tileIndex.Item2 + offset;

                    if (newRow >= 0 && newRow < tiles.GetLength(0) && newCol >= 0 && newCol < tiles.GetLength(1))
                    {
                        Tile newTile = tiles[newRow, newCol];
                        Piece pieceOnTile = newTile.CurrentPiece;

                        if (pieceOnTile != null && pieceOnTile.Owner != Owner)
                        {
                            moves.Add(newTile);
                        }
                    }
                }
            }
            return moves;
        }

        private bool IsStartingPosition(int row)
        {
            // Define the row index where the pawn's initial double move is allowed
            // For white pawns, it's row 1; for black pawns, it's the second-to-last row.
            return (Owner == CurrentTile.Board.WhitePlayer && row == 1) || (Owner == CurrentTile.Board.BlackPlayer && row == 6);
        }
    }
}
