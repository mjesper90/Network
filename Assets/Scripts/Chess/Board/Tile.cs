using UnityEngine;
using Chess.Pieces;
using System;

namespace Chess
{
    public class Tile : MonoBehaviour
    {
        public Piece CurrentPiece;
        public Board Board;

        private int _x, _y;
        private Color _baseColor;
        private Renderer _renderer;

        // Initialize is called from Board
        public void Initialize(int x, int y, Board board)
        {
            this.Board = board;

            transform.position = new Vector3((float)x, 0.2f, (float)y);
            _x = x;
            _y = y;

            _baseColor = BaseColor();
            _renderer = GetComponent<Renderer>();
        }

        private Color BaseColor()
        {
            Color color = Color.white;
            if (_x % 2 == 0)
            {
                if (_y % 2 == 0)
                {
                    color = Color.white;
                }
                else
                {
                    color = Color.black;
                }
            }
            else
            {
                if (_y % 2 == 0)
                {
                    color = Color.black;
                }
                else
                {
                    color = Color.white;
                }
            }
            GetComponent<Renderer>().material.color = color;
            return color;
        }

        public Tuple<int, int> GetCoordinates()
        {
            return new Tuple<int, int>(_x, _y);
        }

        public void ResetColor()
        {
            GetComponent<Renderer>().material.color = _baseColor;
        }

        public void OnMouseDown()
        {
            Board.IC.ClickTile(this);
        }

        public void HighlightTile(Color color)
        {
            _renderer.material.color = color;
        }

        public void SetPiece(Piece piece)
        {
            CurrentPiece = piece;
            if (CurrentPiece != null)
            {
                CurrentPiece.MoveTo(this);
            }
        }
    }
}