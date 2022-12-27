using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

namespace UI.DesignSystem
{
    public class UISlider : MonoBehaviour
    {
        public event UnityAction<float> OnValueChanged;
        public Slider sliderContainer;
        public Text labelContainer;
        public TextMeshProUGUI valueText;
        public SoundBoard soundBoard;

        private Color lowColor = new Color(0, 0, 0, 255);
        private Color highColor = new Color(255, 255, 255, 255);

        public string label
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

        public float value
        {
            get
            {
                return sliderContainer.value;
            }
            set
            {
                sliderContainer.value = value;
                valueText.text = $"{value.ToString()}%";
            }
        }


        private void Start()
        {
            sliderContainer.onValueChanged.AddListener(delegate
            {
                soundBoard.PlaySound(soundBoard.SliderTick);
                if (value > 45)
                {
                    valueText.color = highColor;
                }
                else
                {
                    valueText.color = lowColor;
                }
                OnValueChanged?.Invoke(value);
            });

            valueText.text = $"{value.ToString()}%";
            if (value > 45)
            {
                valueText.color = highColor;
            }
            else
            {
                valueText.color = lowColor;
            }
        }
    }
}