using System;
using System.Collections;
using UnityEngine;

namespace AmongUs.Players
{
    public abstract class APlayer : MonoBehaviour, Interactable
    {
        public bool IsLocal = false;
        public bool LoggedIn = false;
        public string MatchId;
        public string Username;

        protected Rigidbody _rb;

        public void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public abstract void Interact(GameObject Interactor);

        //Lerp movement
        public virtual void LerpMovementAndRotation(Vector3 vector3, float y)
        {
            StartCoroutine(LerpMovementAndRotationCoroutine(vector3, y, CONSTANTS.ServerSpeed));
        }

        //Lerp movement coroutine
        public virtual IEnumerator LerpMovementAndRotationCoroutine(Vector3 vector3, float y, float time)
        {
            float elapsedTime = 0;
            Vector3 startingPos = transform.position;
            Quaternion startingRot = transform.rotation;
            while (elapsedTime < time)
            {
                transform.position = Vector3.Lerp(startingPos, vector3, (elapsedTime / time));
                transform.rotation = Quaternion.Lerp(startingRot, Quaternion.Euler(0, y, 0), (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.position = vector3;
            transform.rotation = Quaternion.Euler(0, y, 0);
        }

        public void Update()
        {
            if (IsLocal)
            {
                Movement();
                Rotation();
                Interact();
            }
        }

        protected abstract void Interact();

        protected abstract void Movement();

        protected abstract void Rotation();
    }
}