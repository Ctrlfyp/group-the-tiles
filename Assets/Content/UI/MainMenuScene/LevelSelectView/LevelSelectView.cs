using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LevelSelectView : MonoBehaviour
    {
        public new Camera camera;
        public LevelBackground background;
        public GameGrid grid;
        public Canvas levelSelectCanvas;
        public GameInputSystem.GameInputSystem gameInputSystem;
        public DialogSystem dialogSystem;
        public LevelInfoPopupMenu levelInfoPopupMenu;
        public bool levelUnlocked;

        [HideInInspector]
        public float mapMinX, mapMaxX, mapMinY, mapMaxY;
        public bool lockX, lockY;

        private int latestUnlockedLevel = 0;

        private void Awake()
        {
            mapMinX = background.transform.position.x - background.GetComponent<SpriteRenderer>().bounds.size.x / 2f;
            mapMaxX = background.transform.position.x + background.GetComponent<SpriteRenderer>().bounds.size.x / 2f;
            mapMinY = 0;
            mapMaxY = background.transform.position.y + background.GetComponent<SpriteRenderer>().bounds.size.y / 2f;
        }

        private void OnEnable()
        {
            UI.PersistentTopBar topBar = ComponentUtility.topBar;
            topBar.SetTopBarMenuMode("Level Select");
        }

        private void Start()
        {
            gameInputSystem.SetupNewManager(nameof(GameInputSystem.LevelSelectIdleManager), this);

            latestUnlockedLevel = GameManager.saveManager.GetLatestLevelId() + 1;
            Vector3? latestLevelButton = null;

            // get all LevelButton components in levelSelectCanvas and check if it's locked
            LevelButton[] levelButtons = levelSelectCanvas.GetComponentsInChildren<LevelButton>();
            foreach (LevelButton levelButton in levelButtons)
            {
                int levelId = int.Parse(levelButton.levelId);
                if (levelUnlocked || latestUnlockedLevel + 1 >= levelId)
                {
                    levelButton.isLocked = false;
                }
                else
                {
                    levelButton.isLocked = true;
                    levelButton.button.interactable = false;
                }

                if (levelId == latestUnlockedLevel)
                {
                    latestLevelButton = levelButton.transform.position;
                }

                levelButton.lockLayer.SetActive(levelButton.isLocked);
            }

            // TODO: Move Camera to latestUnlockedLevel
            // if (latestLevelButton != null)
            // {
            //     Vector3 targetPosition = (Vector3)latestLevelButton;
            //     PanCamera(new Vector3(targetPosition.x, targetPosition.y, camera.transform.position.z));
            // }

        }

        public void SelectLevel(LevelButton button)
        {
            //Vector2 levelPosition = button.gameObject.GetComponent<RectTransform>().anchoredPosition;
            //MoveCameraToCenter(new Vector3(levelPosition.x, levelPosition.y, -1));
            levelInfoPopupMenu.SetLevel(button);
            levelInfoPopupMenu.gameObject.SetActive(true);
        }

        public void MoveCamera(Vector3 position)
        {
            camera.transform.position = ClampCamera(position);
        }

        private void MoveCameraToCenter(Vector3 position)
        {
            MoveCamera(position + new Vector3(-camera.orthographicSize * camera.aspect, camera.orthographicSize));
        }

        private Vector3 ClampCamera(Vector3 targetPosition)
        {
            float camHeight = camera.orthographicSize;
            float camWidth = camera.orthographicSize * camera.aspect;

            float minX = mapMinX + camWidth;
            float maxX = mapMaxX - camWidth;
            float minY = mapMinY + camHeight;
            float maxY = mapMaxY - camHeight;

            float newX = lockX ? camera.transform.position.x : Mathf.Clamp(targetPosition.x, minX, maxX);
            float newY = lockY ? camera.transform.position.y : Mathf.Clamp(targetPosition.y, minY, maxY);

            return new Vector3(newX, newY, targetPosition.z);
        }
    }
}