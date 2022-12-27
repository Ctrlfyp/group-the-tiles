using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelInfo : MonoBehaviour
    {
        public Text timeLeftText;
        public Text movesLeftText;
        public Text scoreText;

        private int movesLeft;

        private void OnEnable()
        {
            GameScene.onMoveMade += UpdateInfo;
        }

        private void OnDisable()
        {
            GameScene.onMoveMade -= UpdateInfo;
        }

        private void UpdateInfo(int currentMoves)
        {
            movesLeft = currentMoves;
            // movesLeftText.text = "Moves Left: " + currentMoves.ToString();
        }

        private void Update()
        {
            GameScene gameScene = ComponentUtility.gameScene;
            if (gameScene)
            {
                TimeSpan timespan = TimeSpan.FromSeconds(gameScene.currentTimeRemaining);
                movesLeftText.text = TranslationSystem.GetText("UIGame", "InfoMovesLeft", new object[] { movesLeft });
                scoreText.text = TranslationSystem.GetText("UIGame", "InfoScore", new object[] { gameScene.currentPoints });
                timeLeftText.text = TranslationSystem.GetText("UIGame", "InfoTimeRemaining", new object[] { timespan.ToString(@"m\:ss") });
            }
        }

        public void SetLevel(GameLevel gameLevel)
        {
            if (gameLevel == null)
            {
                movesLeftText.gameObject.SetActive(false);
                timeLeftText.gameObject.SetActive(false);
                return;
            }

            switch (gameLevel.gameMode)
            {
                case GameLevel.GAME_MODE.CLASSIC:
                    movesLeftText.gameObject.SetActive(true);
                    timeLeftText.gameObject.SetActive(false);
                    break;
                case GameLevel.GAME_MODE.ENDURANCE:
                    movesLeftText.gameObject.SetActive(false);
                    timeLeftText.gameObject.SetActive(true);
                    break;
                case GameLevel.GAME_MODE.RACE:
                    movesLeftText.gameObject.SetActive(false);
                    timeLeftText.gameObject.SetActive(true);
                    break;
                case GameLevel.GAME_MODE.ZEN:
                    movesLeftText.gameObject.SetActive(false);
                    timeLeftText.gameObject.SetActive(false);
                    break;
            }
        }
    }
}