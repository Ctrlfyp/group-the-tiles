using System;
using UnityEngine;

namespace GameInputSystem
{
    public class GenericInputSelector : MonoBehaviour, GameInputSelector
    {
        public event EventHandler InputOccured;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                InputOccured?.Invoke(this, EventArgs.Empty);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                InputOccured?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}