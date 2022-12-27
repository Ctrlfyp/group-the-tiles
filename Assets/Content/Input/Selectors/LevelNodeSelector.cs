using System.Collections.Generic;
using UI;
using System;
using UnityEngine;

namespace GameInputSystem
{
    public class LevelNodeSelector : GameInputSelector
    {
        private List<LevelButton> levels;

        public event EventHandler LevelExit;
        public event EventHandler LevelOver;
        public event EventHandler LevelDown;
        public event EventHandler LevelUp;

        public LevelNodeSelector()
        {
            levels = new List<LevelButton>();
        }

        private void OnLevelOver(object sender, EventArgs args)
        {
            LevelOver?.Invoke(sender, EventArgs.Empty);

            if (Input.GetMouseButtonDown(1))
            {
                LevelDown?.Invoke(sender, EventArgs.Empty);
            }
        }

        private void OnLevelExit(object sender, EventArgs args)
        {
            LevelExit?.Invoke(sender, EventArgs.Empty);
        }

        private void OnLevelDown(object sender, EventArgs args)
        {
            LevelDown?.Invoke(sender, EventArgs.Empty);
        }

        private void OnLevelUp(object sender, EventArgs args)
        {
            LevelUp?.Invoke(sender, EventArgs.Empty);
        }

        public void SetLevels(List<LevelButton> levelButtons)
        {
            foreach (LevelButton component in levels)
            {
                component.LevelDown -= OnLevelDown;
                component.LevelOver -= OnLevelOver;
                component.LevelExit -= OnLevelExit;
                component.LevelUp -= OnLevelUp;
            }
            levels.Clear();

            foreach (LevelButton component in levelButtons)
            {
                levels.Add(component);
                component.LevelDown += OnLevelDown;
                component.LevelOver += OnLevelOver;
                component.LevelExit += OnLevelExit;
                component.LevelUp += OnLevelUp;
            }
        }
    }
}
