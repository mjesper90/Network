using Chess.Pieces;
using UnityEngine;

namespace Chess
{
    public class InputController : MonoBehaviour
    {
        private Piece _selectedPiece;
        private Tile _lastClickedTile;
        private Board _board;

        public void Initialize(Board board)
        {
            _board = board;
        }

        public void ClickTile(Tile tile)
        {
            if (_selectedPiece != null)
            {
                if (_selectedPiece.PossibleMoves().Contains(tile))
                {
                    // Reset colors for all possible moves of the selected piece
                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.ResetColor();
                    }

                    // Perform the move
                    _board.NextMove(_selectedPiece, tile);

                    // Reset selected piece and last clicked tile
                    _selectedPiece = null;
                    _lastClickedTile = null;
                }
                else if (tile.CurrentPiece != null && tile.CurrentPiece.Owner == _board.GetActivePlayer())
                {
                    // Reset colors for all possible moves of the selected piece
                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.ResetColor();
                    }

                    // Select the new piece and highlight its possible moves
                    _selectedPiece = tile.CurrentPiece;
                    _lastClickedTile = tile;

                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.HighlightTile(Color.green);
                    }
                }
            }
            else if (tile.CurrentPiece != null && tile.CurrentPiece.Owner == _board.GetActivePlayer())
            {
                // Set the selected piece and highlight its possible moves
                _selectedPiece = tile.CurrentPiece;
                _lastClickedTile = tile;

                foreach (Tile t in _selectedPiece.PossibleMoves())
                {
                    t.HighlightTile(Color.green);
                }
            }
            else
            {
                if (tile.CurrentPiece != null && tile.CurrentPiece.Owner == _board.GetActivePlayer())
                {
                    // Set the selected piece and highlight its possible moves
                    _selectedPiece = tile.CurrentPiece;
                    _lastClickedTile = tile;

                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.HighlightTile(Color.green);
                    }
                }
            }
        }

        public void Update()
        {
            // Deselect
            if (Input.GetMouseButtonDown(1))
            {
                if (_selectedPiece != null)
                {
                    // Reset colors for all possible moves of the selected piece
                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.ResetColor();
                    }
                }
                _selectedPiece = null;
                _lastClickedTile = null;
            }
        }
    }
}