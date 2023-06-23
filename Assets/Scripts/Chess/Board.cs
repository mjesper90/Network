using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board : MonoBehaviour
    {
        // public List<Piece> ActivePieces = new List<Piece>();

        // Start is called before the first frame update
        void Start()
        {
            

            // Spawn in the different pieces using the TilesToVector method
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 vector3 = hit.point;

                    int[] ints = Vector3ToTile(vector3);

                    print(ints[0] + " " + ints[1]);
                }
            }
        }

        /*public Piece CheckTile()
        {
            // Check for if there is a piece on this tile and return said piece

            return null;
        }*/

        public Vector3 TileToVector(int[] TileCoordinate)
        {
            return new Vector3((float)TileCoordinate[0] * 1.14f, 0.01f, (float)TileCoordinate[1] * 1.14f);
        }

        public int[] Vector3ToTile(Vector3 Position)
        {
            int x = (int)Mathf.Round((Position.x / 1.14f) * 1f);
            int z = (int)Mathf.Round((Position.z / 1.14f) * 1f);

            int[] Tile = new int[2] { x, z };

            return Tile;
        }
    }
}