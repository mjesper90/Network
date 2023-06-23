using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstTask : MonoBehaviour, Interactable
{
    public void Interact(GameObject Interactor)
    {
        Debug.Log("Interacted with " + gameObject.name + " by " + Interactor.name);
        OpenTaskWindow();
    }

    public void OpenTaskWindow()
    {
        
    }
}
