using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class SplashScene : MonoBehaviour
    {

        public UI.DesignSystem.UIButton playButton;
        public GameObject backgroundPanel;
        public UI.DesignSystem.UIDrawInput drawingPanel;
        public AudioClip bgm;
        private AudioManager audioManager;


        private void Start()
        {
            audioManager = ComponentUtility.audioManager;
            audioManager.PlayMusic(bgm);

            playButton.OnClick += OnPlayButtonClicked;
            drawingPanel.ImportDrawing();
        }

        public void OnPlayButtonClicked(UI.DesignSystem.UIButton button)
        {
            drawingPanel.ExportDrawing();
            ComponentUtility.LoadScene("Assets/Content/UI/MainMenuScene/MainMenuScene.unity");
        }
    }
}
