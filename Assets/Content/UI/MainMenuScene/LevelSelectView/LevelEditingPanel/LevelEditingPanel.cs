using System.Collections;
using System.Linq;
using UI.DesignSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelEditingPanel : MonoBehaviour
    {
        private GameLevel currentLevel;

        public UIInputField createIdField;
        public UIDropdown modeDropdown;
        public UIDropdown shapeDropdown;
        public UISlider widthSlider;
        public UISlider heightSlider;

        public UIInputField editIdField;

        public void OnCloseButtonPressed()
        {
            enabled = false;
            gameObject.SetActive(false);
        }

        public void OnCreateButtonPressed()
        {
            if (createIdField.text.Equals(""))
            {
                return;
            }

            enabled = false;
            GameLevel gameLevel = new GameLevel();
            gameLevel.id = createIdField.text;
            gameLevel.gameMode = (GameLevel.GAME_MODE)modeDropdown.value;
            gameLevel.board.gameBoardShape = (GAME_BOARD_SHAPE)shapeDropdown.value;
            gameLevel.board.size.x = widthSlider.value;
            gameLevel.board.size.y = heightSlider.value;

            gameLevel.board.tiles.Add(new Vector2(0, 0), new GameTile());
            currentLevel = gameLevel;
            StartCoroutine(OpenLevel());
        }

        public void OnEditButtonPressed()
        {
            if (editIdField.text.Equals(""))
            {
                return;
            }

            currentLevel = GameManager.dataManager.LoadGameLevelFromId(editIdField.text);
            if (currentLevel != null)
            {
                Debug.Log(currentLevel.board.size);
                StartCoroutine(OpenLevel());
            }
        }

        public void OnEditLevelButtonPressed()
        {
            enabled = !enabled;
            gameObject.SetActive(!gameObject.activeSelf);
        }

        private IEnumerator OpenLevel()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Assets/Content/Game/GameScene/GameScene.unity", LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            GameScene gameScene = scene.GetRootGameObjects().First(obj => { return obj.name.Equals("GameScene"); }).GetComponent<GameScene>();
            gameScene.inEditorLevelId = "";
            gameScene.inEditingMode = true;
            gameScene.SetGameLevel(currentLevel);
        }
    }
}
