using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UI.DesignSystem
{
    public class UIDropdown : MonoBehaviour
    {
        public event UnityAction<int> OnValueChanged;
        public Dropdown dropdownContainer;
        public Text labelContainer;
        public SoundBoard soundBoard;

        private bool isEnabled = false;

        public int value
        {
            get
            {
                return dropdownContainer.value;
            }
            set
            {
                dropdownContainer.value = value;
            }
        }

        private void Start()
        {
            dropdownContainer.onValueChanged.AddListener(delegate
            {
                soundBoard.PlayOneShotSound(soundBoard.OnClick);
                OnValueChanged?.Invoke(value);
            });

        }

        public void SetOptions(List<string> options, int selected = -1)
        {
            dropdownContainer.ClearOptions();
            dropdownContainer.AddOptions(options);
            if (selected >= 0)
            {
                value = selected;
            }
        }

        private void DropdownOpened()
        {
            soundBoard.PlayOneShotSound(soundBoard.MenuAppear);
            Image image = dropdownContainer.GetComponent<Image>();
            image.enabled = true;
        }

        private void DropdownClosed()
        {
            soundBoard.PlayOneShotSound(soundBoard.MenuDisappear);
            Image image = dropdownContainer.GetComponent<Image>();
            image.enabled = false;
        }

        void Update()
        {
            if (dropdownContainer.transform.childCount > 3)
            {
                // where there are more than 2 then that means it's open
                if (!isEnabled)
                {
                    isEnabled = true;
                    DropdownOpened();
                }
            }
            else
            {
                // otherwise it's closed
                if (isEnabled)
                {
                    isEnabled = false;
                    DropdownClosed();
                }
            }
        }
    }
}