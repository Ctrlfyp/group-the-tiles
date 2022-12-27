using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

namespace UI.DesignSystem
{
    public class TabSystem : MonoBehaviour
    {

        public event UnityAction<int> OnTabChanged;
        public float minButtonWidth = 300f;
        public float minButtonHeight = 300f;
        public UI.DesignSystem.UIButton tabButtonPrefab;
        public RectTransform tabScrollContainer;
        public RectTransform tabButtonContainer;
        public RectTransform tabContentContainer;
        public List<UI.DesignSystem.TabSystemEditorListField> tabList;


        private int selectedTabIndex = 0;
        public int selectedTab
        {
            get
            {
                return selectedTabIndex;
            }
            set
            {
                selectedTabIndex = value;
            }
        }

        void Start()
        {
            // remove all child from container
            foreach (Transform child in tabButtonContainer)
            {
                Destroy(child.gameObject);
            }


            // calculate tab size based on container size
            float padding = 10;
            float tabWidth = (tabScrollContainer.rect.width - (padding * (tabList.Count - 1))) / tabList.Count;
            if (tabWidth < minButtonWidth) tabWidth = minButtonWidth;

            float tabHeight = tabScrollContainer.rect.height;
            if (tabHeight < minButtonHeight) tabHeight = minButtonHeight;

            for (int i = 0; i < tabList.Count; i++)
            {
                // Tab Button
                UI.DesignSystem.UIButton tabButton = Instantiate(tabButtonPrefab, tabButtonContainer);
                if (tabList[i].isImage)
                {
                    tabButton.SetImage(tabList[i].titleImage, tabWidth, tabHeight);
                }
                else
                {
                    tabButton.SetText(tabList[i].title);
                }

                tabButton.OnClick += OnTabButtonClick;
                // get tabbutton's LayoutElement
                LayoutElement layoutElement = tabButton.GetComponent<LayoutElement>();
                layoutElement.minWidth = tabWidth;
                layoutElement.flexibleWidth = 1;

                // Tab Content
                GameObject tabContent = tabList[i].content;
                tabContent.SetActive(i == 0 ? true : false);
            }
        }

        private void OnTabButtonClick(UI.DesignSystem.UIButton button)
        {
            // kill current tab
            tabList[selectedTabIndex].content.SetActive(false);

            // let the new tab appear
            int index = 0;
            foreach (Transform child in tabButtonContainer)
            {
                if (child.gameObject == button.gameObject)
                {
                    selectedTabIndex = index;
                    break;
                }
                index++;
            }

            tabList[selectedTabIndex].content.SetActive(true);
            OnTabChanged?.Invoke(selectedTabIndex);
        }

    }
}