using Chess.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board : MonoBehaviour
    {
        public Tile[,] Tiles = new Tile[8, 8];
        public Player WhitePlayer;
        public Player BlackPlayer;
        
        /* true = white, false = black */
        public bool WhitesTurn = false;

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
            SpawnPlayers();
            SpawnTiles();
            SpawnPieces();
        }

        public void SpawnPlayers()
        {
            WhitePlayer = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessPlayerPrefab)).GetComponent<Player>();
            WhitePlayer.gameObject.name = "White Player";
            BlackPlayer = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessPlayerPrefab)).GetComponent<Player>();
            BlackPlayer.gameObject.name = "Black Player";
            Transform playerParent = new GameObject("Players").transform;
            WhitePlayer.transform.parent = playerParent;
            BlackPlayer.transform.parent = playerParent;
            playerParent.parent = transform;

        }

        public void ResetBoard()
        {
            WhitePlayer.Pieces.Clear();
            BlackPlayer.Pieces.Clear();
            DestroyPieces();
            DestroyTiles();
            SpawnTiles();
            SpawnPieces();
        }

        private void DestroyTiles()
        {
            foreach (Tile tile in Tiles)
            {
                Destroy(tile.gameObject);
            }
            Tiles = new Tile[8, 8];
        }

        private void DestroyPieces()
        {
            foreach (Piece piece in ActivePieces)
            {
                Destroy(piece.gameObject);
            }
            ActivePieces = new List<Piece>();
        }

        private void SpawnTiles()
        {
            // Create tile parent
            GameObject tileParent = new GameObject("Tiles");
            tileParent.transform.parent = transform;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    GameObject go = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessTilePrefab));
                    go.name = $"Tile {i}, {j}";
                    go.transform.parent = tileParent.transform;
                    go.GetComponent<Tile>().Initialize(i, j, this);
                    Tiles[i, j] = go.GetComponent<Tile>();
                }
            }
        }

        private void SpawnPieces()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int value = StandardSetup[i, j];

                    if (value != 0)
                    {
                        // Spawn piece
                        GameObject go = Instantiate(PiecePrefabs[value - 1]);
                        Player owner = (i < 4) ? WhitePlayer : BlackPlayer;
                        go.transform.parent = owner.transform;

                        // Set piece owner
                        Piece p = go.GetComponent<Piece>();
                        p.Owner = owner;
                        owner.Pieces.Add(p);

                        // Set piece tile
                        Tile tile = Tiles[i, j];
                        tile.SetPiece(p);
                        ActivePieces.Add(p);
                    }
                }
            }
        }
    }
}