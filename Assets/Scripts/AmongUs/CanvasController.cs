using System.Collections;
using System.Collections.Generic;
using AmongUs.Tasks;
using UnityEngine;

namespace AmongUs.GameControl
{
    public class CanvasController : MonoBehaviour
    {
        public static CanvasController Instance { get; private set; }
        public TaskOverlay TaskOverlay { get; private set; }

        public void Awake()
        {
            Instance = this;
            TaskOverlay = GetComponentInChildren<TaskOverlay>();
        }

        public void Start()
        {
            TaskOverlay.gameObject.SetActive(false);
        }
    }
}