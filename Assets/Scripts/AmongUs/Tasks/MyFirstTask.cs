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
        /*
        GameObject taskWindow = Instantiate(Resources.Load("TaskWindow")) as GameObject;
        taskWindow.transform.SetParent(GameObject.Find("Canvas").transform);
        taskWindow.transform.localPosition = new Vector3(0, 0, 0);
        taskWindow.transform.localScale = new Vector3(1, 1, 1);
        */
    }
}
