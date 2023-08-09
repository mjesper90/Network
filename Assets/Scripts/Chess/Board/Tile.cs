using UnityEngine;
using Chess.Pieces;

namespace Chess
{
    public class Tile : MonoBehaviour
    {
        private int _x, _y;
        private Color _color;
        private Piece _piece;


        // Initialize is called from Board
        public void Initialize(int x, int y)
        {
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
            transform.position = new Vector3(x, 0, y);
            _x = x;
            _y = y;
        }

        void OnMouseDown()
        {
            Debug.Log("Tile clicked at coordinate (" + _x + ", " + _y + ")");
        }

        public void SetPiece(Piece piece)
        {
            _piece = piece;
        }

        public Piece GetPiece()
        {
            return _piece;
        }

        public void ChangeColor()
        {
            _color = Color.white;
            GetComponent<Renderer>().material.color = _color;
        }
    }
}