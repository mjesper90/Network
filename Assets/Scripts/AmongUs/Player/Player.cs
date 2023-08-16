using System.Collections;
using AmongUs.Players;
using UnityEngine;

namespace AmongUs
{
    public class Player : APlayer
    {
        protected override void Movement()
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

        protected override void Interact()
        {
            //Interact with object
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, CONSTANTS.AmongUsInteractDistance))
                {
                    //Check for self hit
                    if (hit.transform.gameObject == gameObject)
                    {
                        return;
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<Interactable>()?.Interact(gameObject);
                    }
                }
            }
        }

        protected override void Rotation()
        {
            //Rotate transform with mouse
            float mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up * mouseX);
        }

        public override void Interact(GameObject Interactor)
        {
            Debug.Log("Interacted with " + gameObject.name + " by " + Interactor.name);
        }
    }
}