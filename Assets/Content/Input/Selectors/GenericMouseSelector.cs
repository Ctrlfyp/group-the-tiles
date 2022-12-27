using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameInputSystem
{
    public class MouseEventArgs : EventArgs
    {
        public int mouseButton { get; set; }
        public Vector2 mouseLocation { get; set; }
        public MouseEventArgs(int mouseButton, Vector2 mouseLocation)
        {
            this.mouseButton = mouseButton;
            this.mouseLocation = mouseLocation;
        }
    }

    public class GenericMouseSelector : MonoBehaviour, GameInputSelector
    {
        private Vector2 lastTouched;
        public event EventHandler<MouseEventArgs> MouseButtonDown;
        public event EventHandler<MouseEventArgs> MouseButtonUp;
        public event EventHandler<Vector2> MouseMoved;
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                MouseButtonDown?.Invoke(this, new MouseEventArgs(0, Input.mousePosition));
            }
            if (Input.GetMouseButtonDown(1))
            {
                MouseButtonDown?.Invoke(this, new MouseEventArgs(1, Input.mousePosition));
            }
            if (Input.GetMouseButtonUp(0))
            {
                MouseButtonUp?.Invoke(this, new MouseEventArgs(0, Input.mousePosition));
            }
            if (Input.GetMouseButtonUp(1))
            {
                MouseButtonUp?.Invoke(this, new MouseEventArgs(1, Input.mousePosition));
            }
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                MouseMoved?.Invoke(this, Input.mousePosition);
            }
        }
    }
}
