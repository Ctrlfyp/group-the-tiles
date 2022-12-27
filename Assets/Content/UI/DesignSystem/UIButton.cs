using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.DesignSystem
{
    public class UIButton : MonoBehaviour, IPointerEnterHandler
    {
        public event UnityAction<UI.DesignSystem.UIButton> OnClick;
        public Button buttonContainer;
        public Text labelContainer;
        public Image imageContainer;
        public bool isImage = false;
        public SoundBoard soundBoard;

        public string text
        {
            get
            {
                return this.labelContainer.text;
            }
            set
            {
                this.labelContainer.text = value;
            }
        }

        private void Start()
        {
            buttonContainer.onClick.AddListener(delegate
            {
                soundBoard.PlayOneShotSound(soundBoard.OnClick);
                OnClick?.Invoke(this);
            });

            if (isImage)
            {
                // change to image mode
                imageContainer.gameObject.SetActive(true);
                labelContainer.gameObject.SetActive(false);
                buttonContainer.targetGraphic = imageContainer;

                // get image component within buttonContainer and disable it
                buttonContainer.GetComponent<Image>().enabled = false;
            }
            else
            {
                // change to text mode
                // imageContainer.gameObject.SetActive(false);
                // labelContainer.gameObject.SetActive(true);
                // buttonContainer.targetGraphic = labelContainer;
            }
        }

        public void SetImage(Sprite sprite, float width = -1, float height = -1)
        {
            imageContainer.sprite = sprite;
            isImage = true;
            imageContainer.gameObject.SetActive(true);
            labelContainer.gameObject.SetActive(false);
            buttonContainer.targetGraphic = imageContainer;
            buttonContainer.GetComponent<Image>().enabled = false;

            if (width > 0 && height > 0)
            {
                imageContainer.rectTransform.sizeDelta = new Vector2(width, height);
            }
        }

        public void SetText(string text)
        {
            labelContainer.text = text;
            isImage = false;
            imageContainer.gameObject.SetActive(false);
            labelContainer.gameObject.SetActive(true);
            buttonContainer.targetGraphic = labelContainer;
            buttonContainer.GetComponent<Image>().enabled = false;
        }


        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            soundBoard.PlayOneShotSound(soundBoard.OnHover);
        }

    }
}
