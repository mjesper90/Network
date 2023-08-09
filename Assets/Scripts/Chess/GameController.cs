using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess.Pieces;

namespace Chess
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        public Piece SelectedPiece;
        public Tile ClickedTile;
        public Player WhitePlayer;
        public Player BlackPlayer;

        /* true = white, false = black */
        public bool WhitesTurn = false;

        public List<Board> Boards = new List<Board>();

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void Start()
        {
            WhitePlayer = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessPlayerPrefab)).GetComponent<Player>();
            BlackPlayer = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessPlayerPrefab)).GetComponent<Player>();
            Board b = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessBoardPrefab)).GetComponent<Board>();
            Boards.Add(b);
            b.Initialize();
        }

        public void EndTurn()
        {

        }
    }
}