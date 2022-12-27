using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace UI
{
    public class LevelButton : MonoBehaviour
    {
        public string levelId;
        public Button button;
        public GameObject lockLayer;
        public event EventHandler LevelOver;
        public event EventHandler LevelExit;
        public event EventHandler LevelDown;
        public event EventHandler LevelUp;

        [HideInInspector]
        private GameLevel currentLevel;
        [HideInInspector]
        private GameSave.LevelProgress currentLevelProgress;

        public Text pointText;
        public UI.LevelInfoDisplay levelInfoDisplay;
        public GameObject starContainer;
        private UI.StarObject[] starObjects;
        public bool isLocked = true;

        private void Start()
        {
            button.onClick.AddListener(OnMouseDown);
        }

        void OnMouseOver()
        {
            LevelOver?.Invoke(this, EventArgs.Empty);
        }

        void OnMouseExit()
        {
            LevelExit?.Invoke(this, EventArgs.Empty);
        }

        void OnMouseDown()
        {
            LevelDown?.Invoke(this, EventArgs.Empty);
        }

        void OnMouseUp()
        {
            LevelUp?.Invoke(this, EventArgs.Empty);
        }

        void OnBecameVisible()
        {
            // only load level when it's in view
            if (currentLevel == null)
            {
                loadLevel();
            }
        }

        private void loadLevel()
        {
            currentLevel = GameManager.dataManager.LoadGameLevelFromId(levelId);

            if (GameManager.saveManager.currentSave.playedData.ContainsKey(currentLevel.id))
            {
                currentLevelProgress = GameManager.saveManager.currentSave.playedData[currentLevel.id];
                pointText.text = TranslationSystem.GetText("UILevelSelect", "PreviousBest", new object[] { currentLevelProgress.bestScore });
            }

            if (starObjects == null || starObjects.Length < 3)
            {
                // get the stars
                starObjects = starContainer.GetComponentsInChildren<UI.StarObject>();
            }

            foreach (KeyValuePair<int, int> scoreRequired in currentLevel.difficultyScoreRequired)
            {
                if (currentLevelProgress != null)
                {
                    starObjects[scoreRequired.Key - 1].SetStar(scoreRequired.Value <= currentLevelProgress.bestScore, scoreRequired.Value.ToString());
                }
                else
                {
                    starObjects[scoreRequired.Key - 1].SetStar(false, scoreRequired.Value.ToString());
                }
            }

            levelInfoDisplay.SetLevelInfoPanel(currentLevel.board.gameBoardShape, currentLevel.gameMode, true);
        }
    }
}
