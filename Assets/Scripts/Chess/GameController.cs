using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess.Pieces;

namespace Chess
{
    public class GameController : MonoBehaviour
    {
        public static Piece SelectedPiece;
        public Player WhitePlayer;
        public Player BlackPlayer;
        
        /* true = white, false = black */
        public bool WhitesTurn = false;

        private Board _board;

        public void Start()
        {
            _board = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessBoardPrefab)).GetComponent<Board>();
            _board.Initialize();

            /*WhitePlayer = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessPlayerPrefab)).GetComponent<Player>();

            BlackPlayer = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessPlayerPrefab)).GetComponent<Player>();*/
        }

        public void EndTurn()
        {

        }
    }
}