using System;
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

            if (CurrentTile != null)
            {
                Tuple<int, int> tileIndex = CurrentTile.GetCoordinates();
                int row = tileIndex.Item1;
                int col = tileIndex.Item2;

                // Check possible moves in all eight directions: up, down, left, right, and diagonals
                moves.AddRange(GetValidMovesInDirection(row, col, 1, 0));   // Up
                moves.AddRange(GetValidMovesInDirection(row, col, -1, 0));  // Down
                moves.AddRange(GetValidMovesInDirection(row, col, 0, 1));   // Right
                moves.AddRange(GetValidMovesInDirection(row, col, 0, -1));  // Left
                moves.AddRange(GetValidMovesInDirection(row, col, 1, 1));   // Diagonal up-right
                moves.AddRange(GetValidMovesInDirection(row, col, 1, -1));  // Diagonal up-left
                moves.AddRange(GetValidMovesInDirection(row, col, -1, 1));  // Diagonal down-right
                moves.AddRange(GetValidMovesInDirection(row, col, -1, -1)); // Diagonal down-left
            }

            return moves;
        }

        // Helper method to get valid moves in a specific direction
        private List<Tile> GetValidMovesInDirection(int startRow, int startCol, int rowDelta, int colDelta)
        {
            Tile[,] tiles = CurrentTile.Board.Tiles;
            List<Tile> validMoves = new List<Tile>();

            int newRow = startRow + rowDelta;
            int newCol = startCol + colDelta;

            while (IsInBounds(newRow, newCol))
            {
                Tile tile = tiles[newRow, newCol];

                if (tile.CurrentPiece == null)
                {
                    validMoves.Add(tile);
                }
                else if (tile.CurrentPiece.Owner != Owner)
                {
                    validMoves.Add(tile);
                    break; // Stop if an opponent's piece is encountered
                }
                else
                {
                    break; // Stop if own piece is encountered
                }

                newRow += rowDelta;
                newCol += colDelta;
            }

            return validMoves;
        }

        // Helper method to check if a position is within the board bounds
        private bool IsInBounds(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }
    }
}
