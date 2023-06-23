using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board : MonoBehaviour
    {
        public List<Piece> ActivePieces = new List<Piece>();

        // Start is called before the first frame update
        void Start()
        {
            // Spawn in the different pieces using the TilesToVector method
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool SpaceOccupied(int[] Tile)
        {
            Vector3 TestPos = TileToVector(Tile);

            foreach (var piece in ActivePieces)
            {
                if (piece.position = TestPos) 
                {
                    return false;
                }
            }

            return true;
        }

        public Vector3 TileToVector(int[] TileCoordinate)
        {
            return new Vector3((float)TileCoordinate[0] * 1.14f, 0.01f, (float)TileCoordinate[1] * 1.14f);
        }

        public int[] Vector3ToTile(Vector3 Position)
        {
            return int[Math.Round((int)(Position.X / 1.14f), 0), Math.Round((int)(Position.Z/1.14f), 0)];
        }
    }
}