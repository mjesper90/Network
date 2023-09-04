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
                Tile[,] tiles = CurrentTile.Board.Tiles;
                Tuple<int, int> tileIndex = CurrentTile.GetCoordinates();

                bool maxMove = false;
                // check to the right
                for (int i = tileIndex.Item1; i < 8; i++)
                {
                    if (maxMove == false)
                    {
                        Tile newTile = tiles[tileIndex.Item2, i];

                        if (newTile.CurrentPiece == null)
                        {
                            moves.Add(newTile);
                        }
                        else
                        {
                            maxMove = true;
                        }
                    }
                }

                maxMove = false;

                // check to the left
                for (int i = tileIndex.Item1; i >= 0; i--)
                {
                    if (maxMove == false)
                    {
                        Tile newTile = tiles[tileIndex.Item2, i];

                        if (newTile.CurrentPiece == null)
                        {
                            moves.Add(newTile);
                        }
                        else
                        {
                            maxMove = true;
                        }
                    }
                }

                maxMove = false;

                // check forward
                for (int i = tileIndex.Item2; i < 8; i++)
                {
                    if (maxMove == false)
                    {
                        Tile newTile = tiles[i, tileIndex.Item1];

                        if (newTile.CurrentPiece == null)
                        {
                            moves.Add(newTile);
                        }
                        else
                        {
                            maxMove = true;
                        }
                    }
                }

                maxMove = false;
                
                // check backward
                for (int i = tileIndex.Item2; i >= 0; i--)
                {
                    if (maxMove == false)
                    {
                        Tile newTile = tiles[i, tileIndex.Item1];

                        if (newTile.CurrentPiece == null)
                        {
                            moves.Add(newTile);
                        }
                        else
                        {
                            maxMove = true;
                        }
                    }
                }

                maxMove = false;
            }

            return moves;
        }
    }
}