using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StarObject : MonoBehaviour
    {

        public Text label;
        public Image starEnabledImage;
        public Image starDisabledImage;

        public float speed = 5f;

        private bool isStarEnabled;

        public void SetStar(bool enabled, string label = null)
        {
            if (label != null)
            {
                this.label.text = label;
            }
            else
            {
                this.label.gameObject.SetActive(false);
            }

            isStarEnabled = enabled;
        }

        public void ResetStar()
        {
            starEnabledImage.color = Color.clear;
        }

        public void OnEnable()
        {
            ResetStar();
        }

        public void Update()
        {
            // Play a shitty animation when it's enabled
            if (isStarEnabled)
            {
                if (Mathf.Abs(1f - starEnabledImage.color.a) > 0.01f)
                {
                    starEnabledImage.color = Color.Lerp(starEnabledImage.color, Color.white, Time.deltaTime * speed);
                }
            }
            else
            {
                ResetStar();
            }
        }

    }
}
