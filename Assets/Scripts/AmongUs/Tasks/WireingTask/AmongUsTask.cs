using System;
using System.Collections;
using System.Collections.Generic;
using AmongUs.GameControl;
using UnityEngine;
using UnityEngine.UI;

namespace AmongUs.Tasks
{
    public abstract class AmongUsTask : MonoBehaviour
    {
        public event Action TaskCompleted;

        protected virtual void OnTaskCompleted()
        {
            TaskCompleted?.Invoke();
        }
    }
}