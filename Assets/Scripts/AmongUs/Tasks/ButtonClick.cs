using System.Collections;
using System.Collections.Generic;
using AmongUs.GameControl;
using UnityEngine;

namespace AmongUs.Tasks
{
    public class ButtonClick : MonoBehaviour, Interactable
    {
        public void Interact(GameObject Interactor)
        {
            Debug.Log("Interacted with " + gameObject.name + " by " + Interactor.name);
            OpenTaskWindow();
        }

        public void OpenTaskWindow()
        {
            CanvasController.Instance.TaskOverlay.gameObject.SetActive(true);
        }
    }
}