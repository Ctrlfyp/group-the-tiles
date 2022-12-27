using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
    {

        public Image itemImage;
        public Text itemText;
        public SoundBoard soundBoard;

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            soundBoard.PlayOneShotSound(soundBoard.OnHover);
        }

        public void OnPointerClick(PointerEventData pointerEventData)
        {
            soundBoard.PlayOneShotSound(soundBoard.OnClick);
        }


        public void SetItem(ShopCatalogueItem item, bool isOwned)
        {
            // find ItemContainer
            GameObject itemContainer = transform.Find("ItemContainer").gameObject;
            foreach (Transform child in itemContainer.transform)
            {

                if (child.name == "ItemImage")
                {
                    child.GetComponent<Image>().sprite = Resources.Load<Sprite>("Shop/Icons/" + item.image);
                    if (isOwned)
                    {
                        Color color = child.GetComponent<Image>().color;
                        color.a = 0.25f;
                        child.GetComponent<Image>().color = color;
                    }
                }

                if (child.name == "ItemDescription")
                {
                    child.GetComponent<Text>().text = "<b>" + item.name + "</b>\n" + item.description;
                    if (isOwned)
                    {
                        Color color = child.GetComponent<Text>().color;
                        color.a = 0.25f;
                        child.GetComponent<Text>().color = color;
                    }
                }
                else if (child.name == "ItemPriceContainer")
                {
                    // price text
                    Text priceText = child.GetComponentInChildren<Text>();
                    priceText.text = item.price.ToString();

                    if (isOwned)
                    {
                        // text
                        Color color = priceText.color;
                        color.a = 0.25f;
                        priceText.color = color;

                        // images
                        Image priceIcon = child.Find("StarPriceContainer").GetComponentInChildren<Image>();
                        Image buyButton = child.Find("FakeBuyButton").GetComponent<Image>();

                        // change alpha
                        color = priceIcon.color;
                        color.a = 0.25f;
                        priceIcon.color = color;
                        buyButton.color = color;
                    }
                }
            }
        }

    }
}
