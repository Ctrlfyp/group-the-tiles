using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using UnityEngine.Events;
using TMPro;

namespace UI
{
    public class PersistentTopBar : MonoBehaviour
    {

        public TextMeshProUGUI starCountText;
        public SettingsMenu settingsMenu;
        public StarMenu starMenu;

        public GameObject menuModeContainer;
        public GameObject gameModeContainer;

        public bool isGameMode = false;

        public Text menuTitleText;
        public Text levelTitleText;
        // public GameObject starContainer;

        public UI.StarProgressBar starProgressBar;

        public UI.LevelInfoDisplay levelInfoDisplay;

        // private UI.StarObject[] starObjects;
        private List<string> menuTitleList;

        void Start()
        {
            menuTitleList = new List<string>();
            CloseMenu();
            SetTopBarMenuMode();
            // starObjects = starContainer.GetComponentsInChildren<UI.StarObject>();
        }

        protected void OnDisable()
        {
            if (isGameMode)
            {
                GameScene.onMoveMade -= UpdateInfo;
            }
        }

        protected void OnEnable()
        {
            if (isGameMode)
            {
                GameScene.onMoveMade += UpdateInfo;
            }
        }

        public void SetTopBarMenuMode(string title = null)
        {
            if (title != null)
            {
                menuTitleText.text = title;
            }
            menuModeContainer.SetActive(true);
            gameModeContainer.SetActive(false);
        }

        public void SetTopBarGameMode()
        {
            GameScene gameScene = ComponentUtility.gameScene;
            menuModeContainer.SetActive(false);
            gameModeContainer.SetActive(true);

            isGameMode = true;

            levelInfoDisplay.SetLevelInfoPanel(gameScene.currentLevel.board.gameBoardShape, gameScene.currentLevel.gameMode, true);
            levelTitleText.text = $"Level {gameScene.currentLevel.id}";
            GameScene.onMoveMade += UpdateInfo;

            starProgressBar.Init(gameScene.currentLevel);
        }

        public void UpdateInfo(int currentMoves)
        {
            GameScene gameScene = ComponentUtility.gameScene;
            // if (!gameScene.inEditingMode)
            // {
            //     foreach (KeyValuePair<int, int> scoreRequired in gameScene.currentLevel.difficultyScoreRequired)
            //     {
            //         starObjects[scoreRequired.Key - 1].SetStar(scoreRequired.Value <= gameScene.currentPoints);
            //     }
            // }
            starProgressBar.UpdateStatus(gameScene.currentPoints);
        }


        private void SaveMenuTitleAndSet(string title)
        {
            menuTitleList.Add(menuTitleText.text);
            menuTitleText.text = title;
        }

        public void CloseMenu(bool clearTitle = false)
        {
            settingsMenu.gameObject.SetActive(false);
            starMenu.gameObject.SetActive(false);

            // bootback the original menu title
            if (clearTitle && menuTitleList.Count > 0)
            {
                menuTitleText.text = menuTitleList[0];
                menuTitleList.Clear();
            }
        }

        public void OnSettingsMenuClicked()
        {

            // Toggle game pause and unpause
            if (isGameMode)
            {
                GameScene gameScene = ComponentUtility.gameScene;
                if (gameScene && settingsMenu.gameObject.activeSelf)
                {
                    gameScene.ResumeGame();
                }
                else if (gameScene && !settingsMenu.gameObject.activeSelf)
                {
                    gameScene.PauseGame();
                }
            }

            // Toggle menu display
            starMenu.gameObject.SetActive(false);
            if (settingsMenu.gameObject.activeSelf)
            {
                CloseMenu(true);
            }
            else
            {
                SaveMenuTitleAndSet("Settings");
                settingsMenu.gameObject.SetActive(true);
            }
        }

        public void OnStarMenuClicked()
        {
            settingsMenu.gameObject.SetActive(false);
            if (starMenu.gameObject.activeSelf)
            {
                CloseMenu(true);
            }
            else
            {
                SaveMenuTitleAndSet("Cash");
                starMenu.gameObject.SetActive(true);
            }
        }

        void Update()
        {
            if (!isGameMode)
            {
                starCountText.text = GameManager.saveManager.currentSave.currency.ToString();
            }
        }
    }

}
