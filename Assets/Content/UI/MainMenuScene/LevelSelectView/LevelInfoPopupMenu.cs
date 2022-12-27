using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LevelInfoPopupMenu : MonoBehaviour
    {
        public Text levelText;
        public Text previousBestText;

        public GameObject starContainer;
        private UI.StarObject[] starObjects;

        public UI.LevelInfoDisplay levelInfoDisplay;

        public GameObject levelRestrictionsLayoutContainer;
        public GameObject levelRestrictionTextPrefab;

        public SoundBoard soundBoard;

        [HideInInspector]
        private GameLevel currentLevel;
        [HideInInspector]
        private GameSave.LevelProgress currentLevelProgress;


        public void SetLevel(LevelButton levelButton)
        {
            ResetLevel();
            currentLevel = GameManager.dataManager.LoadGameLevelFromId(levelButton.levelId);

            // set board and mode info
            levelInfoDisplay.SetLevelInfoPanel(currentLevel.board.gameBoardShape, currentLevel.gameMode);

            levelText.text = $"Level {currentLevel.id}";

            if (GameManager.saveManager.currentSave.playedData.ContainsKey(currentLevel.id))
            {
                currentLevelProgress = GameManager.saveManager.currentSave.playedData[currentLevel.id];
                previousBestText.text = TranslationSystem.GetText("UILevelSelect", "PreviousBest", new object[] { currentLevelProgress.bestScore });
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

            // TODO update this to show more info in a tree hierarchy
            foreach (LevelRestriction restriction in currentLevel.restrictions.restrictions)
            {
                if (restriction is LevelRestrictionClause)
                {
                    GameObject newText = Instantiate(levelRestrictionTextPrefab, levelRestrictionsLayoutContainer.transform);
                    Text text = newText.GetComponent<Text>();
                    LevelRestrictionClause restrictionClause = restriction as LevelRestrictionClause;
                    if (restrictionClause is LevelRestrictionClause.GroupCountClause)
                    {
                        LevelRestrictionClause.GroupCountClause groupCountClause = restrictionClause as LevelRestrictionClause.GroupCountClause;
                        text.text = $"Each group must have {groupCountClause.requiredSize} tiles";
                    }
                    else if (restrictionClause is LevelRestrictionClause.GroupMinimumClause)
                    {
                        LevelRestrictionClause.GroupMinimumClause groupMinimumClause = restrictionClause as LevelRestrictionClause.GroupMinimumClause;
                        text.text = $"Each group must have exactly {groupMinimumClause.referenceTileValues.Select(e => $"[{e.Key | e.Value}]")} tiles";
                    }
                    else if (restrictionClause is LevelRestrictionClause.GroupSpecificClause)
                    {
                        LevelRestrictionClause.GroupSpecificClause groupSpecificClause = restrictionClause as LevelRestrictionClause.GroupSpecificClause;
                        text.text = $"Each group must have at least {groupSpecificClause.referenceTileValues.Select(e => $"[{e.Key | e.Value}]")} tiles";
                    }
                    else if (restrictionClause is LevelRestrictionClause.GroupTileCountClause)
                    {
                        LevelRestrictionClause.GroupTileCountClause groupCountClause = restrictionClause as LevelRestrictionClause.GroupTileCountClause;
                        text.text = $"Each Submission must have {groupCountClause.requiredSize} groups";
                    }
                }
            }

            soundBoard.PlayOneShotSound(soundBoard.MenuAppear);
        }

        public void OnPlayButtonPressed()
        {
            if (currentLevel.id.Equals(""))
            {
                gameObject.SetActive(false);
            }
            OpenLevel();
        }

        public void OnCloseButtonPressed()
        {
            soundBoard.PlayOneShotSound(soundBoard.MenuDisappear);
            gameObject.SetActive(false);
        }

        private void ResetLevel()
        {
            currentLevelProgress = null;
            levelText.text = $"Level-id:";
            ComponentUtility.RemoveChildren(levelRestrictionsLayoutContainer.transform);
            previousBestText.text = TranslationSystem.GetText("UILevelSelect", "PreviousBest", new object[] { 0 });
            foreach (Transform t in ComponentUtility.GetChildren(starContainer.transform))
            {
                t.GetComponentInChildren<Image>().color = new Color(1, 1, 1);
            }
        }

        private void OpenLevel()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            ComponentUtility.LoadScene("Assets/Content/Game/GameScene/GameScene.unity");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            GameScene gameScene = ComponentUtility.gameScene;
            if (gameScene != null)
            {
                gameScene.inEditingMode = false;
                gameScene.inEditorLevelId = currentLevel.id;
                gameScene.SetGameLevel(currentLevel);
                Debug.Log(currentLevel.id);
            }
        }
    }
}