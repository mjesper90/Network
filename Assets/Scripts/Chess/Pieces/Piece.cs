using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Pieces
{
    public abstract class Piece : MonoBehaviour, Moveable
    {
        public abstract List<Tile> PossibleMoves();
        public virtual void MoveTo(Tile tile)
        {
            StartCoroutine(lerpMovement(transform.position, tile, 1f));
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
