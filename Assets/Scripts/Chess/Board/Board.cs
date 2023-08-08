using Chess.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board : MonoBehaviour
    {
        public GameObject Selector;
        public List<Tile> Tiles;

        public List<GameObject> PiecePrefabs = new List<GameObject>();

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
                    Tiles.Add(go.GetComponent<Tile>());
                }
            }
        }


        public void StartPosition(int owner)
        {
            SetupPiece(4, 4, 0, owner);

            for (int i = 0; i < 8; i++)
            {
                SetupPiece(0, i, 1, owner);
            }
        }

        /* Piece value:
        0: Pawn
        1: Rook
        2: Knight
        3: Bishop
        4: Queen
        5: King */
        public void SetupPiece(int value, int x, int y, int owner)
        {
            GameObject piece = Instantiate(PiecePrefabs[value]);
            SetLocation(piece, x, y);
        }

        public static void SetLocation(GameObject piece, int x, int y)
        {
            int[] pos = new int[2];

            pos[0] = x;
            pos[1] = y;

            //piece.transform.position = Board.TileToVector(pos);
        }
    }
}