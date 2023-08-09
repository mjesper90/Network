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
            transform.position = new Vector3((float)x, 0.2f, (float)y);
            _x = x;
            _y = y;
        }

        void OnMouseDown()
        {
            if (_piece == null)
            {
                GetComponent<Renderer>().material.color = Color.red;
            }

            if (true == true)
            {

            }

            GameController.SelectedPiece = GetPiece();

            GetComponent<Renderer>().material.color = Color.green;


        }

        public void SetPiece(Piece piece)
        {
            _piece = piece;
            Piece p = piece.GetComponent<Piece>();
            GameObject go2 = p.gameObject;
            go2.transform.position = transform.position + new Vector3(0.5f, 0, 0.5f);
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