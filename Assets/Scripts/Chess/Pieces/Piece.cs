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

        public void ChangeColor(Color color)
        {
            GetComponent<Renderer>().material.color = color;
        }

        public void ApplyTexture(Texture2D texture)
        {
            GetComponent<Renderer>().material.mainTexture = texture;
        }

        public virtual void MoveTo(Tile tile)
        {
            if (tile != null)
            {
                if (CurrentTile != null)
                {
                    CurrentTile.SetPiece(null);
                }
                CurrentTile = tile;

                if (tile.CurrentPiece != null)
                {
                    //Attack piece on tile
                }
                tile.CurrentPiece = this;
                StartCoroutine(lerpMovement(tile.transform.position, 1f));
            }
        }

        private IEnumerator lerpMovement(Vector3 targetPosition, float duration)
        {
            Vector3 startingPosition = transform.position;
            float elapsedTime = 0f;
            targetPosition = targetPosition + new Vector3(0, 0.5f, 0);

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition; // Ensure final position is accurate
        }
    }
}
