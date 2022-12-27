using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Localization;

namespace UI
{

    public class ShopView : MonoBehaviour
    {

        public enum SHOP_ITEM_TYPE
        {
            THEME, MUSIC, LEVEL, NFT
        }

        public GameObject shopItemPrefab;
        public RectTransform themePanel;
        public RectTransform musicPanel;
        public RectTransform levelPanel;
        public RectTransform nftPanel;
        public SoundBoard soundBoard;

        private List<VerticalLayoutGroup> contentList;

        private ShopCatalogue shopCatalogue;

        public MainMenuScene mainMenuScene;

        public GameObject dialoguePrefab;

        void Start()
        {
            // get script from parent to tell user their stuff's been bought
            mainMenuScene = GameObject.Find("MainMenuScene").GetComponent<MainMenuScene>();

            TextAsset json = Resources.Load<TextAsset>("Shop/Catalogue");
            ShopCatalogue catalogue = JsonConvert.DeserializeObject<ShopCatalogue>(json.text);
            shopCatalogue = catalogue;

            // get the scroll containers
            contentList = new List<VerticalLayoutGroup>();
            contentList.Add(themePanel.GetComponentInChildren<VerticalLayoutGroup>());
            contentList.Add(musicPanel.GetComponentInChildren<VerticalLayoutGroup>());
            contentList.Add(levelPanel.GetComponentInChildren<VerticalLayoutGroup>());
            contentList.Add(nftPanel.GetComponentInChildren<VerticalLayoutGroup>());

            RenderShopList();
        }

        private void OnEnable()
        {
            UI.PersistentTopBar topBar = ComponentUtility.topBar;
            topBar.SetTopBarMenuMode("Shop");
        }

        private void RenderShopList()
        {
            HashSet<string> unlockedItemIds = GameManager.saveManager.GetAllUnlockedItemIds();

            // loop through contentList and add them to list
            for (int i = 0; i < contentList.Count; i++)
            {
                VerticalLayoutGroup content = contentList[i];
                ComponentUtility.RemoveChildren(content.transform);

                // loop through shopCatalogue
                foreach (ShopCatalogueItem item in shopCatalogue.shopItems)
                {
                    if (item.type == (int)(SHOP_ITEM_TYPE)i)
                    {
                        // create shop item
                        bool isOwned = unlockedItemIds.Contains(item.id);
                        GameObject shopItem = Instantiate(this.shopItemPrefab, content.transform);
                        renderList(shopItem, item, isOwned);
                    }
                }
            }
        }

        private void renderList(GameObject shopItem, ShopCatalogueItem item, bool isOwned)
        {
            UI.ShopItem shopItemScript = shopItem.GetComponent<UI.ShopItem>();
            shopItemScript.SetItem(item, isOwned);
            shopItem.GetComponent<Button>().onClick.AddListener(() => { ConfirmBuyItem(item); });
            if (isOwned)
            {
                shopItem.GetComponent<Button>().enabled = false;
            }
        }

        private void ConfirmBuyItem(ShopCatalogueItem item)
        {
            if (GameManager.saveManager.GetCurrency() >= item.price)
            {
                string promptMessage = TranslationSystem.GetText("UIMainMenu", "StoreBuyConfirmDialogue");
                mainMenuScene.ShowDialogPrompt(promptMessage, () => { BuyItem(item); }, () => { });
            }
            else
            {
                string promptMessage = TranslationSystem.GetText("UIMainMenu", "StoreBuyPoorDialogue");
                mainMenuScene.ShowDialogPrompt(promptMessage);
            }
        }

        private void BuyItem(ShopCatalogueItem item)
        {
            if (GameManager.saveManager.GetCurrency() >= item.price)
            {
                soundBoard.PlayOneShotSound(soundBoard.Purchase);
                GameManager.saveManager.BuyItem(item);
                // GameManager.dataManager.AddItem(item.name);

                // Show prompt for buying the item
                string promptMessage = TranslationSystem.GetText("UIMainMenu", "StoreBoughtDialogue", new object[] { item.name, item.price.ToString() });
                mainMenuScene.ShowDialogPrompt(promptMessage);

                RenderShopList();
            }
        }

    }
}
