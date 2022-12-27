using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UI.DesignSystem;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace UI
{
    public class GameSceneLevelEditingPanel : MonoBehaviour
    {
        public GameScene gameScene;
        public UIInputField createIdField;
        public UIDropdown modeDropdown;
        public UIDropdown shapeDropdown;
        public UISlider widthSlider;
        public UISlider heightSlider;
        public UISlider timeSlider;
        public UISlider moveSlider;
        public UISlider colourSlider;
        public UIInputField oneStarPointsField;
        public UIInputField twoStarPointsField;
        public UIInputField threeStarPointsField;
        public GameObject rulesLevelLayout;

        public LevelRestrictionGroupContainer levelRestrictionGroupContainerPrefab;

        public void SaveLevel()
        {
            GameLevel currentLevel = gameScene.currentLevel;
            Debug.Log(currentLevel.GetHashCode());

            if (currentLevel == null)
            {
                return;
            }

            string levelJson = JsonConvert.SerializeObject(currentLevel, Constants.serializerSettings);
            string fileName = $"Assets/Resources/Levels/Level{currentLevel.id}.json";
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(levelJson);
                }
            }
        }

        public void OnCloseButtonPressed()
        {
            enabled = false;
            gameObject.SetActive(false);
        }

        public void OnSettingsButtonPressed()
        {
            enabled = !enabled;
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void SetLevel(GameLevel level)
        {
            createIdField.text = level.id;
            modeDropdown.value = (int)level.gameMode;
            shapeDropdown.value = (int)level.board.gameBoardShape;
            widthSlider.value = level.board.size.x;
            heightSlider.value = level.board.size.y;
            timeSlider.value = level.totalTime;
            moveSlider.value = level.totalMoves;
            colourSlider.value = level.totalColours;
            oneStarPointsField.text = level.difficultyScoreRequired[1].ToString();
            twoStarPointsField.text = level.difficultyScoreRequired[2].ToString();
            threeStarPointsField.text = level.difficultyScoreRequired[3].ToString();

            ComponentUtility.RemoveChildren(rulesLevelLayout.transform);
            GameObject gameObject = Instantiate(new GameObject(), rulesLevelLayout.transform);
            gameObject.name = "GroupHolder";
            gameObject.AddComponent<RectTransform>();
            LevelRestrictionGroupContainer levelRestrictionGroupContainer = Instantiate(levelRestrictionGroupContainerPrefab, gameObject.transform);
            levelRestrictionGroupContainer.SetRestrictionGroup(level, level.restrictions);
            levelRestrictionGroupContainer.levelRestrictionGroupContainerPrefab = levelRestrictionGroupContainerPrefab;
        }

        public void OnIdChanged(string id)
        {
            GameLevel level = gameScene.currentLevel;
            level.id = id;
            ResetLevel();
        }

        public void OnModeChanged(int mode)
        {
            GameLevel level = gameScene.currentLevel;
            level.gameMode = (GameLevel.GAME_MODE)mode;
            ResetLevel();
        }

        public void OnShapeChanged(int shape)
        {
            GameLevel level = gameScene.currentLevel;
            level.board.gameBoardShape = (GAME_BOARD_SHAPE)shape;
            ResetLevel();
        }

        public void OnWidthChanged(float width)
        {
            GameLevel level = gameScene.currentLevel;
            level.board.size.x = width;
            level.board.tiles = level.board.tiles.Where(x => x.Value.location.x <= width - 1).ToDictionary(x => x.Key, x=> x.Value);
            ResetLevel();
        }

        public void OnHeightChanged(float height)
        {
            GameLevel level = gameScene.currentLevel;
            level.board.size.y = height;
            level.board.tiles = level.board.tiles.Where(x => x.Value.location.y <= height - 1).ToDictionary(x => x.Key, x => x.Value);
            ResetLevel();
        }

        public void OnTimeChanged(float time)
        {
            GameLevel level = gameScene.currentLevel;
            level.totalTime = (int)time;
            ResetLevel();
        }
        public void OnMovesChanged(float totalMoves)
        {
            GameLevel level = gameScene.currentLevel;
            level.totalMoves = (int)totalMoves;
            ResetLevel();
        }

        public void OnTotalColoursChanged(float totalColours)
        {
            GameLevel level = gameScene.currentLevel;
            level.totalColours = (int)totalColours;
            ResetLevel();
        }

        public void OnStarOneThresholdChanged(string pointsThreshold)
        {
            OnStarThresholdChanged(1, float.Parse(pointsThreshold));
        }

        public void OnStarTwoThresholdChanged(string pointsThreshold)
        {
            OnStarThresholdChanged(2, float.Parse(pointsThreshold));
        }

        public void OnStarThreeThresholdChanged(string pointsThreshold)
        {
            OnStarThresholdChanged(3, float.Parse(pointsThreshold));
        }

        private void OnStarThresholdChanged(int difficulty, float pointsThreshold)
        {
            GameLevel level = gameScene.currentLevel;
            level.difficultyScoreRequired[difficulty] = (int)pointsThreshold;
            ResetLevel();
        }

        public void ResetLevel()
        {
            if (gameScene.currentLevel != null)
            {
                gameScene.SetGameLevel(gameScene.currentLevel);
            }
        }
    }
}