using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI
{
    public class DialogSystem : MonoBehaviour
    {
        public Text displayText;
        public Button yesButton;
        public Button noButton;
        public SoundBoard soundBoard;
        public bool isActive = false;
        private bool isInit = false;

        public void SetDisplayText(string text, UnityAction handleYes = null, UnityAction handleNo = null)
        {
            displayText.text = text;

            if (handleYes != null)
            {
                yesButton.gameObject.SetActive(true);
                yesButton.onClick.AddListener(handleYes);
                yesButton.onClick.AddListener(HandleDestroy);
            }

            if (handleNo != null)
            {
                noButton.gameObject.SetActive(true);
                noButton.onClick.AddListener(handleNo);
                noButton.onClick.AddListener(HandleDestroy);
            }

            // Just show ok button when no action is provided
            if (handleYes == null && handleNo == null)
            {
                yesButton.gameObject.SetActive(true);
                yesButton.onClick.AddListener(HandleDestroy);
            }
            isInit = true;
        }

        private void HandleDestroy()
        {
            soundBoard.PlaySound(soundBoard.MenuDisappear);
            Destroy(gameObject);
        }

        private void Update()
        {
            // if selfActive
            if (gameObject.activeSelf && !isActive && isInit)
            {
                if (soundBoard)
                {
                    soundBoard.PlaySound(soundBoard.MenuAppear);
                    isActive = true;
                }
            }
        }
    }
}