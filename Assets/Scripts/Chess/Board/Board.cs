using Chess.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board : MonoBehaviour //, IBoard
    {
        public GameObject Selector;
        public static Tile[,] Tiles = new Tile[8, 8];

        /* Piece value:
        0: Blank
        1: Pawn
        2: Rook
        3: Knight
        4: Bishop
        5: Queen
        6: King */
        public List<GameObject> PiecePrefabs = new List<GameObject>();

        public static int[,] StandardSetup = new int[8, 8]
        {
            {2, 3, 4, 5, 6, 4, 3, 2},
            {1, 1, 1, 1, 1, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 1, 1, 1, 1, 1},
            {2, 3, 4, 5, 6, 4, 3, 2}
        };

        public List<Piece> ActivePieces = new List<Piece>();

        // Spawn 8x8 board and pieces
        // This method is called from GameController
        public void Initialize()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessTilePrefab));
                    go.transform.parent = transform;
                    go.GetComponent<Tile>().Initialize(i, j);
                    Tiles[i, j] = go.GetComponent<Tile>();
                }
            }

            SpawnPieces();
        }

        public void SpawnPieces()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int value = StandardSetup[i, j];

                    if (value != 0)
                    {
                        GameObject piece = Instantiate(PiecePrefabs[value]);

                        Tile tile = Tiles[i, j];

                        tile.SetPiece(piece);
                    }
                }
            }
        }

        public void SetupPiece(int value, int x, int y, Player owner)
        {
            GameObject piece = Instantiate(PiecePrefabs[value]);
        }
    }
}