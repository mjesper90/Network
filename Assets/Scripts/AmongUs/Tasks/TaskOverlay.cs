using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AmongUs.Tasks
{
    public class TaskOverlay : MonoBehaviour
    {
        public static TaskOverlay Instance { get; private set; }
        public Button CancelButton;

        public void Awake()
        {
            Instance = this;
            foreach (Button button in GetComponentsInChildren<Button>())
            {
                if (button.name == "CancelButton")
                {
                    CancelButton = button;
                }
            }
            CancelButton.onClick.AddListener(CancelTask);
        }

        private void CancelTask()
        {
            gameObject.SetActive(false);
        }
    }
}