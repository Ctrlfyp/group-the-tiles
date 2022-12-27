using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.DesignSystem
{
    public class UIProgressBar : MonoBehaviour
    {
        public Slider slider;
        public float speed = 5f;

        private float targetValue = 0;

        public float value
        {
            get
            {
                return slider.value;
            }
            set
            {
                targetValue = value;
                slider.value = value;
            }
        }

        private void Update()
        {
            if (Mathf.Abs(targetValue - slider.value) > 0.01f)
            {
                slider.value = Mathf.Lerp(slider.value, targetValue, Time.deltaTime * speed);
            }
        }

        public void SetProgress(float value)
        {
            targetValue = value;
        }


    }
}