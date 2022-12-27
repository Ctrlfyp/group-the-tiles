using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using System.Linq;

namespace UI
{
    public class GameOverPanel : MonoBehaviour
    {
        public Image gameEndScreenImage;
        public Sprite winImage;
        public Sprite loseImage;

        public UI.LevelInfoDisplay levelInfoDisplay;
        public Text pointsText;
        public GameObject starContainer;
        private UI.StarObject[] starObjects;

        public SoundBoard soundBoard;

        public UI.DesignSystem.UIButton nextLevelButton;
        public UI.DesignSystem.UIButton quitButton;
        public UI.DesignSystem.UIButton buyMoveButton;

        public GameObject GameSummary;

        private GameLevel currentLevel;
        private GameSave.LevelProgress currentLevelProgress;
        private int currentPoints;


        public void InitGameOverPanel(GameLevel currentLevel, GameSave.LevelProgress currentLevelProgress, int currentPoints)
        {
            this.currentLevel = currentLevel;
            this.currentLevelProgress = currentLevelProgress;
            this.currentPoints = currentPoints;
            SetPanelInfo();
        }

        private void SetPanelInfo()
        {

            bool isWin = false;

            // update Star Display
            if (starObjects == null || starObjects.Length < 3)
            {
                // get the stars
                starObjects = starContainer.GetComponentsInChildren<UI.StarObject>();
            }

            foreach (KeyValuePair<int, int> scoreRequired in currentLevel.difficultyScoreRequired)
            {
                if (currentLevelProgress != null)
                {
                    Image currentImage = starContainer.transform.GetChild(scoreRequired.Key - 1).GetComponentInChildren<Image>();
                    if (scoreRequired.Value <= currentPoints)
                    {
                        // assuming any star means you passed
                        isWin = true;
                    }
                    starObjects[scoreRequired.Key - 1].SetStar(scoreRequired.Value <= currentLevelProgress.bestScore, scoreRequired.Value.ToString());
                }
                else
                {
                    starObjects[scoreRequired.Key - 1].SetStar(false, scoreRequired.Value.ToString());
                }
            }

            // set score and level info
            levelInfoDisplay.SetLevelInfoPanel(currentLevel.board.gameBoardShape, currentLevel.gameMode);
            pointsText.text = TranslationSystem.GetText("UIGame", "GameOverPointsMessage", new object[] { currentPoints });

            if (isWin)
            {
                // title text
                gameEndScreenImage.sprite = winImage;

                // show the next level button
                nextLevelButton.gameObject.SetActive(true);

                // Set the buttons to their corresponding visibility
                nextLevelButton.gameObject.SetActive(true);
                buyMoveButton.gameObject.SetActive(false);
                quitButton.gameObject.SetActive(false);

                // play sound
                soundBoard.PlaySound(soundBoard.Win);
            }
            else
            {
                // title text
                gameEndScreenImage.sprite = loseImage;

                // Set the buttons to their corresponding visibility
                nextLevelButton.gameObject.SetActive(false);

                // allows the user to buy more moves to continue
                int currentCurrency = GameManager.saveManager.currentSave.currency;
                if (currentCurrency < 1)
                {
                    // guy's broke
                    buyMoveButton.gameObject.SetActive(false);
                    quitButton.gameObject.SetActive(true);
                }
                else
                {
                    // guy can buy moves to continue
                    buyMoveButton.gameObject.SetActive(true);
                    quitButton.gameObject.SetActive(false);
                }

                // play sound
                soundBoard.PlaySound(soundBoard.Lose);
            }

            // show this panel
            gameObject.SetActive(true);
        }

        public void OnBuyMoveButtonClicked()
        {
            GameScene gameScene = ComponentUtility.gameScene;

            // deduct currency
            GameManager.saveManager.RemoveCurrency(1);

            // hide this panel
            gameObject.SetActive(false);
            gameScene.ReviveGame();
        }

        public void OnRetryButtonClicked()
        {
            OpenLevel(false);
        }

        public void OnCloseButtonClicked()
        {
            ComponentUtility.LoadScene("Assets/Content/UI/MainMenuScene/MainMenuScene.unity");
        }

        public void OnNextLevelButtonClicked()
        {
            OpenLevel(true);
        }

        private void OpenLevel(bool next)
        {
            if (next)
            {
                SceneManager.sceneLoaded += OnLoadNextlevel;
            }
            else
            {
                SceneManager.sceneLoaded += OnLoadCurrentlevel;
            }
            ComponentUtility.LoadScene("Assets/Content/Game/GameScene/GameScene.unity");
        }

        private void OnLoadNextlevel(Scene scene, LoadSceneMode loadSceneMode)
        {
            GameScene gameScene = ComponentUtility.gameScene;
            gameScene.SetGameLevel(GameManager.dataManager.LoadGameLevelFromId((int.Parse(currentLevel.id) + 1).ToString()));
        }

        private void OnLoadCurrentlevel(Scene scene, LoadSceneMode loadSceneMode)
        {
            GameScene gameScene = ComponentUtility.gameScene;
            gameScene.ResetAndSetGameLevel();
        }
    }
}
