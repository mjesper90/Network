using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour, Moveable
    {
        public Player Owner = null;
        public Tile CurrentTile = null;

        public abstract List<Tile> PossibleMoves();

    public void Start()
        {
            if (Owner.gameObject.name == "Black Player")
            {
                // This code should theoretically change the piece color for the black player, but doesn't currently work.

                Material material = GetComponent<Renderer>().material;

                string name = material.name;

                name = name.Remove(0, 4);

                material.SetTexture("Red" + name, null);
            }
        }

        public virtual void MoveTo(Tile tile)
        {
            if (tile != null)
            {
                if (CurrentTile != null) CurrentTile.SetPiece(null);
                CurrentTile = tile;
                StartCoroutine(lerpMovement(transform.position, tile, 1f));
            }
        }

        IEnumerator lerpMovement(Vector3 start, Tile tile, float speed)
        {
            float startTime = Time.time;
            Vector3 target = new Vector3(tile.transform.position.x, tile.transform.position.y + 1f, tile.transform.position.z);
            float distance = Vector3.Distance(start, target);
            float fracJourney = 0;
            while (fracJourney < 1)
            {
                float distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / distance;
                transform.position = Vector3.Lerp(start, target, fracJourney);
                yield return null;
            }
        }
    }
}
