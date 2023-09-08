using Chess.Pieces;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board : MonoBehaviour
    {
        public Tile[,] Tiles = new Tile[8, 8];
        public List<BoardLog> MatchLog = new List<BoardLog>();
        public Player WhitePlayer;
        public Player BlackPlayer;

        public InputController IC;

        /* true = white, false = black */
        public bool WhitesTurn = true;

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

        // Spawn 8x8 board and pieces
        // This method is called from GameController
        public void Initialize()
        {
            IC = gameObject.AddComponent<InputController>();
            IC.Initialize(this);
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
            foreach (Piece piece in GetActivePieces())
            {
                Destroy(piece.gameObject);
            }
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
                    Player owner = (i < 4) ? WhitePlayer : BlackPlayer;

                    if (value != 0)
                    {
                        SpawnPiece(i, j, value, owner);
                    }
                }
            }
        }

        public void SpawnPiece(int row, int col, int value, Player owner)
        {
            // Spawn piece
            GameObject go = Instantiate(PiecePrefabs[value - 1]);
            go.name = $"{go.name}".Substring(0, go.name.Length - 7);
            go.transform.parent = owner.transform;

            // Set piece owner
            Piece p = go.GetComponent<Piece>();
            p.Owner = owner;

            // Set piece tile
            Tile tile = Tiles[row, col];
            tile.SetPiece(p);
            owner.Pieces.Add(p);
            if (owner == BlackPlayer)
            {
                string path = CONSTANTS.TextureBlackDir + p.gameObject.name;
                Texture2D txt2d = Resources.Load<Texture2D>(path);
                p.ApplyTexture(txt2d);
            }
            else
            {
                string path = CONSTANTS.TextureWhiteDir + p.gameObject.name;
                Texture2D txt2d = Resources.Load<Texture2D>(path);
                p.ApplyTexture(txt2d);
            }
        }

        public void NextMove(Piece piece, Tile tile)
        {
            if (piece == null || tile == null)
            {
                Debug.Log("NextMove: Invalid move");
                return;
            }
            BoardLog logEntry = new BoardLog(piece.CurrentTile, tile);
            MatchLog.Add(logEntry);
            Debug.Log($"Match log: {MatchLog.Count} " + logEntry.ToString());
            piece.MoveTo(tile);

            WhitesTurn = !WhitesTurn;
        }

        public List<Piece> GetActivePieces()
        {
            List<Piece> pieces = new List<Piece>();
            foreach (Tile tile in Tiles)
            {
                if (tile.CurrentPiece != null)
                {
                    pieces.Add(tile.CurrentPiece);
                }
            }
            return pieces;
        }

        public Player GetActivePlayer()
        {
            return WhitesTurn ? WhitePlayer : BlackPlayer;
        }
    }
}