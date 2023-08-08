using System.Collections;
using System.Collections.Generic;
using AmongUs.GameControl;
using UnityEngine;
using UnityEngine.UI;

namespace AmongUs.Tasks
{
    public class WireingTask : AmongUsTask
    {
        public Slider _slider1, _slider2;

        public void Awake()
        {
            Slider[] Sliders = GetComponentsInChildren<Slider>();
            if (Sliders.Length == 2)
            {
                _slider1 = Sliders[0];
                _slider2 = Sliders[1];
            }
            else
            {
                Debug.LogError("WireingTask has wrong amount of sliders");
            }
        }

        public void Update()
        {
            if (_slider1.value == 1 && _slider2.value == 1)
            {
                _slider1.value = 0;
                _slider2.value = 0;
                OnTaskCompleted();
            }
        }
    }
}