using UnityEngine;
using Chess.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;


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

        void OnMouseDown()
        {
            //Deselect the piece if right click
            if (Input.GetMouseButtonDown(1))
            {
                if (GameController.Instance.SelectedPiece != null)
                {
                    foreach (Tile t in GameController.Instance.SelectedPiece.PossibleMoves())
                    {
                        t.ResetColor();
                    }
                    GameController.Instance.SelectedPiece = null;
                }
                return;
            }

            //Deselect the last clicked tile
            if (GameController.Instance.ClickedTile != null)
            {
                GameController.Instance.ClickedTile.ResetColor();
            }

            //Set the clicked tile
            GameController.Instance.ClickedTile = this;

            //Move the piece if possible
            if (GameController.Instance.SelectedPiece != null)
            {
                foreach (Tile t in GameController.Instance.SelectedPiece.PossibleMoves())
                {
                    t.ResetColor();
                }
                if (GameController.Instance.SelectedPiece.PossibleMoves().Contains(this))
                {
                    GameController.Instance.SelectedPiece.MoveTo(this);
                    GameController.Instance.SelectedPiece = null;
                    return;
                }
            }

            //Select the piece if there is one
            if (CurrentPiece != null)
            {
                //Highlight the possible moves
                GameController.Instance.SelectedPiece = CurrentPiece;
                HighlightTile(Color.green);
                foreach (Tile t in CurrentPiece.PossibleMoves())
                {
                    t.HighlightTile(Color.blue);
                }
                return;
            }
            
            HighlightTile(Color.yellow);
            GameController.Instance.SelectedPiece = null;
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