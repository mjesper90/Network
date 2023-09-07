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
            // If there is a selected piece, check if the tile is a possible move
            if (_selectedPiece != null)
            {
                if (_selectedPiece.PossibleMoves().Contains(tile))
                {
                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.ResetColor();
                    }
                    _board.NextMove(_selectedPiece, tile);
                    _selectedPiece = null;
                    _lastClickedTile = null;
                }
                else
                {
                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.ResetColor();
                    }
                    _selectedPiece = null;
                    _lastClickedTile = null;
                }
            }
            else
            {
                if (tile.CurrentPiece != null && tile.CurrentPiece.Owner == _board.GetActivePlayer())
                {
                    _selectedPiece = tile.CurrentPiece;
                    _lastClickedTile = tile;
                    foreach (Tile t in _selectedPiece.PossibleMoves())
                    {
                        t.HighlightTile(Color.green);
                    }
                }
            }
        }
    }
}