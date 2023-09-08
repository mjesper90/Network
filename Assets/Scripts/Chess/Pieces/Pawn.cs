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

        public override void MoveTo(Tile tile)
        {
            base.MoveTo(tile);

            // Check for promotion
            if (isPromotionPosition(tile.GetCoordinates().Item1))
            {
                Promote();
            }
        }

        private bool IsStartingPosition(int col)
        {
            return (Owner == CurrentTile.Board.WhitePlayer && col == 1) || (Owner == CurrentTile.Board.BlackPlayer && col == 6);
        }

        private bool isPromotionPosition(int col)
        {
            return (Owner == CurrentTile.Board.WhitePlayer && col == 7) || (Owner == CurrentTile.Board.BlackPlayer && col == 0);
        }

        private void Promote()
        {
            CurrentTile.Board.SpawnPiece(CurrentTile.GetCoordinates().Item1, CurrentTile.GetCoordinates().Item2, 5, Owner);
            Destroy(gameObject);
        }
    }
}
