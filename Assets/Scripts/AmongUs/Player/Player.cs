using System;
using System.Collections;
using AmongUs.GameControl;
using UnityEngine;

namespace AmongUs
{
    public class Player : MonoBehaviour
    {
        public string Username;
        public string MatchId;
        public bool IsLocal = false;
        public bool LoggedIn = false;

        private Rigidbody _rb;

        public void Awake()
        {
            _rb = GetComponent<Rigidbody>();
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

        //Lerp movement
        public void LerpMovementAndRotation(Vector3 vector3, float y)
        {
            StartCoroutine(LerpMovementAndRotationCoroutine(vector3, y, CONSTANTS.ServerSpeed));
        }

        //Lerp movement coroutine
        private IEnumerator LerpMovementAndRotationCoroutine(Vector3 vector3, float y, float time)
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

        private void Movement()
        {
            //WASD Movement
            Vector3 movement = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                movement += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                movement -= transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                movement -= transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                movement += transform.right;
            }
            movement.Normalize();
            movement *= CONSTANTS.PlayerSpeed;
            _rb.velocity = new Vector3(movement.x, 0, movement.z);
        }

        private void Interact()
        {
            //Interact with object
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, CONSTANTS.AmongUsInteractDistance))
                {
                    hit.transform.gameObject.GetComponent<Interactable>()?.Interact(gameObject);
                }
            }
        }

        private void Rotation()
        {
            //Rotate transform with mouse
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up * mouseX);
        }
    }
}