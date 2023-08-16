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

        public void OnMouseDown()
        {
            Player activePlayer = Board.ActivePlayer();

            // Deselect the last clicked tile
            if (activePlayer.ClickedTile != null)
            {
                activePlayer.ClickedTile.ResetColor();
            }

            // Set the clicked tile
            activePlayer.ClickedTile = this;

            Piece selectedPiece = activePlayer.SelectedPiece;

            if (selectedPiece != null)
            {
                // Handle piece movement
                foreach (Tile t in selectedPiece.PossibleMoves())
                {
                    t.ResetColor();
                }

                if (selectedPiece.PossibleMoves().Contains(this) && selectedPiece.Owner == activePlayer)
                {
                    Board.NextMove(selectedPiece, this);
                    activePlayer.SelectedPiece = null;
                    return;
                }
            }

            Piece currentPiece = CurrentPiece;

            if (currentPiece != null)
            {
                // Select a piece and highlight its possible moves
                activePlayer.SelectedPiece = currentPiece;
                HighlightTile(Color.green);

                foreach (Tile t in currentPiece.PossibleMoves())
                {
                    t.HighlightTile(Color.blue);
                }
            }
            else
            {
                // No piece selected, highlight tile
                HighlightTile(Color.yellow);
                activePlayer.SelectedPiece = null;
            }
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