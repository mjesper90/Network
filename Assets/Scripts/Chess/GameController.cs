using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chess.Pieces;

namespace Chess
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        public List<Board> Boards = new List<Board>();

        //Singleton
        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        //Instantiate and initialize a board
        public void Start()
        {
            Board b = Instantiate(Resources.Load<GameObject>(CONSTANTS.ChessBoardPrefab)).GetComponent<Board>();
            Boards.Add(b);
            b.Initialize();
        }
    }
}