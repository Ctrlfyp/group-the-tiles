using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System.Collections;

namespace UI
{
    public class MainMenuScene : MonoBehaviour
    {
        public LevelSelectView levelSelectView;
        public ShopView shopView;
        public GameObject dialogSystem;
        public AudioClip mainMenuBGM;
        private AudioManager audioManager;

        public void Start()
        {
            audioManager = ComponentUtility.audioManager;
            audioManager.PlayMusic(mainMenuBGM);
        }

        public void ShowLevelSelectViewButtonPressed()
        {
            ComponentUtility.topBar.CloseMenu();
            levelSelectView.gameObject.SetActive(true);
            shopView.gameObject.SetActive(false);
        }

        public void ShowShopViewButtonPressed()
        {
            ComponentUtility.topBar.CloseMenu();
            levelSelectView.gameObject.SetActive(false);
            shopView.gameObject.SetActive(true);
        }

        public void ShowDialogPrompt(string text, UnityAction yesAction = null, UnityAction noAction = null)
        {
            UI.DialogSystem ds = Instantiate(dialogSystem, GameObject.Find("MainMenuCanvas").GetComponent<Canvas>().rootCanvas.transform).GetComponent<UI.DialogSystem>();
            ds.SetDisplayText(text, yesAction, noAction);
        }

        public void OnPlayButtonClicked()
        {

            SceneManager.LoadSceneAsync("Assets/Content/Game/LevelSelect/LevelSelectScene.unity", LoadSceneMode.Single);
        }

        public void OnQuitButtonClicked()
        {
            Application.Quit();
        }
    }
}
