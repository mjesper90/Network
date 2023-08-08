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
            if (x % 2 == 0)
            {
                if (y % 2 == 0)
                {
                    GetComponent<Renderer>().material.color = Color.white;
                }
                else
                {
                    GetComponent<Renderer>().material.color = Color.black;
                }
            }
            else
            {
                if (y % 2 == 0)
                {
                    GetComponent<Renderer>().material.color = Color.black;
                }
                else
                {
                    GetComponent<Renderer>().material.color = Color.white;
                }
            }

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