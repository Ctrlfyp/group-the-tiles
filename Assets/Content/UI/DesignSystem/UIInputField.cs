using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace UI.DesignSystem
{
    public class UIInputField : MonoBehaviour
    {
        public event UnityAction<string> OnValueChanged;
        public InputField inputField;
        private string lastTextState = "";

        public string text
        {
            get
            {
                return this.inputField.text;
            }
            set
            {
                this.inputField.text = value;
            }
        }

        void Update()
        {
            if (lastTextState != text)
            {
                OnValueChanged?.Invoke(text);
                lastTextState = text;
            }
        }
    }
}