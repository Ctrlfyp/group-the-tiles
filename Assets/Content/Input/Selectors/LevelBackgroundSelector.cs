using System;
using UnityEngine;

namespace GameInputSystem
{
    public class LevelBackgroundSelector : GameInputSelector
    {
        public event EventHandler<MouseEventArgs> MouseButtonDown;
        public event EventHandler<MouseEventArgs> MouseButtonUp;
        public event EventHandler<Vector2> MouseMoved;

        public LevelBackgroundSelector(UI.LevelBackground levelBackground)
        {
            levelBackground.MouseButtonDown += OnMouseDown;
            levelBackground.MouseButtonUp += OnMouseUp;
            levelBackground.MouseMoved += OnMouseMoved;
        }

        private void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseButtonDown?.Invoke(sender, mouseEventArgs);
        }

        private void OnMouseUp(object sender, MouseEventArgs mouseEventArgs)
        {
            MouseButtonUp?.Invoke(sender, mouseEventArgs);
        }

        private void OnMouseMoved(object sender, Vector2 location)
        {
            MouseMoved?.Invoke(sender, location);
        }
    }
}
