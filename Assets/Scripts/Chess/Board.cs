using Chess.Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class Board : MonoBehaviour
    {
        public GameObject Selector;

        public List<GameObject> PiecePrefabs = new List<GameObject>();

        public List<Piece> ActivePieces = new List<Piece>();

        // Start is called before the first frame update
        void Start()
        {
            StartPosition(0);

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
                    CheckHit(hit);
                }
            }
        }

        public void CheckHit(RaycastHit hit)
        {
            Vector3 vector3 = hit.point;

            int[] ints = Vector3ToTile(vector3);

            if (0 <= ints[0] && ints[0] <= 7 && 0 <= ints[1] && ints[1] <= 7)
            {
                

                Selector.transform.position = TileToVector(ints);


            }
        }

        public static Vector3 TileToVector(int[] TileCoordinate)
        {
            return new Vector3((float)TileCoordinate[0] * 1.15f, 0.2f, (float)TileCoordinate[1] * 1.15f);
        }

        public static int[] Vector3ToTile(Vector3 Position)
        {
            int x = (int)Mathf.Round((Position.x / 1.15f) * 1f);
            int z = (int)Mathf.Round((Position.z / 1.15f) * 1f);

            int[] Tile = new int[2] { x, z };

            return Tile;
        }

        public void StartPosition(int owner)
        {
            SetupPiece(4, 4, 0, owner);

            for (int i = 0; i < 8; i++)
            {
                SetupPiece(0, i, 1, owner);
            }
        }

        /* Piece value:
        0: Pawn
        1: Rook
        2: Knight
        3: Bishop
        4: Queen
        5: King */
        public void SetupPiece(int value, int x, int y, int owner)
        {
            GameObject piece = Instantiate(PiecePrefabs[value]);
            SetLocation(piece, x, y);
        }

        public static void SetLocation(GameObject piece, int x, int y)
        {
            int[] pos = new int[2];

            pos[0] = x;
            pos[1] = y;

            piece.transform.position = Board.TileToVector(pos);
        }
    }
}