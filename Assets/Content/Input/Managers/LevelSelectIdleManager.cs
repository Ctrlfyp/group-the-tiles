using System;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameInputSystem
{
    public class LevelSelectIdleManager : GameInputManager
    {
        private bool mouseHeld;
        private LevelSelectView levelSelectView;
        private Vector2 lastMouseVector;
        private Vector3 totalGridDelta;

        public override void OnStart()
        {
            levelSelectView = gameInputSystem.gameObject.GetComponent<LevelSelectView>();
        }
        public override void OnEnd() {}

        public void OnMouseMove(object sender, Vector2 location)
        {
            if (mouseHeld)
            {
                Vector3 posDelta = lastMouseVector - location;
                Vector3 prevCameraPosition = levelSelectView.camera.transform.position;
                lastMouseVector = location;
                levelSelectView.MoveCamera(levelSelectView.camera.transform.position + posDelta);
                totalGridDelta += levelSelectView.camera.transform.position - prevCameraPosition;

                float gridSize = levelSelectView.grid.size;
                if (Math.Abs(totalGridDelta.y) > gridSize || Math.Abs(totalGridDelta.x) > gridSize)
                {
                    if (Math.Abs(totalGridDelta.y) > gridSize)
                    {
                        totalGridDelta.y = levelSelectView.grid.transform.localPosition.y % gridSize;
                        levelSelectView.grid.transform.localPosition = new Vector3(levelSelectView.grid.transform.localPosition.x, -totalGridDelta.y, 1);
                    }
                    if (Math.Abs(totalGridDelta.x) > gridSize)
                    {
                        totalGridDelta.x = levelSelectView.grid.transform.localPosition.x % gridSize;
                        levelSelectView.grid.transform.localPosition = new Vector3(-totalGridDelta.x, levelSelectView.grid.transform.localPosition.y, 1);
                    }

                    levelSelectView.grid.transform.localPosition += this.levelSelectView.camera.transform.position - new Vector3(540, 960, 0);
                }
            }
        }

        public void OnMouseDown(object sender, MouseEventArgs args)
        {
            lastMouseVector = args.mouseLocation;
            if (args.mouseButton == 0)
            {
                mouseHeld = true;
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs args)
        {
            if (args.mouseButton == 0)
            {
                mouseHeld = false;
            }
        }

        public void OnLevelHover(object sender, EventArgs args)
        {

        }

        public void OnLevelExit(object sender, EventArgs args)
        {

        }

        public void OnLevelDown(object sender, EventArgs args)
        {
            LevelButton levelButton = sender as LevelButton;
            levelSelectView.SelectLevel(levelButton);
        }
    }
}
