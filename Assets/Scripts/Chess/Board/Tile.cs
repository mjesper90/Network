using UnityEngine;
using Chess.Pieces;
using System;


namespace Chess
{
    public class Tile : MonoBehaviour
    {
        private int _x, _y;
        private Color _color;
        public Piece _piece;
        public Board Board;

        // Initialize is called from Board
        public void Initialize(int x, int y, Board board)
        {
            this.Board = board;
            _color = Color.black;
            if (x % 2 == 0)
            {
                if (y % 2 == 0)
                {
                    _color = Color.black;
                }
                else
                {
                    _color = Color.white;
                }
            }
            else
            {
                if (y % 2 == 0)
                {
                    _color = Color.white;
                }
                else
                {
                    _color = Color.black;
                }
            }
            GetComponent<Renderer>().material.color = _color;
            transform.position = new Vector3((float)x, 0.2f, (float)y);
            _x = x;
            _y = y;
        }

        public Tuple<int, int> GetCoordinates()
        {
            return new Tuple<int, int>(_x, _y);
        }

        void OnMouseDown()
        {
            GameController.Instance.ClickedTile = this;
            if (_piece != null)
            {
                GameController.Instance.SelectedPiece = _piece;
            }
        }

        public void SetPiece(Piece piece)
        {
            _piece = piece;
            if (_piece != null)
            {
                _piece.MoveTo(this);
            }
        }

        public Piece GetPiece()
        {
            return _piece;
        }

        public void RemovePiece()
        {
            _piece = null;
        }

        public void ChangeColor()
        {
            _color = Color.white;
            GetComponent<Renderer>().material.color = _color;
        }
    }
}