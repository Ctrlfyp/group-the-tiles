using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopCatalogue
{
    public List<ShopCatalogueItem> shopItems;

    public ShopCatalogue()
    {
        shopItems = new List<ShopCatalogueItem>();
    }
}


[Serializable]
public class ShopCatalogueItem
{
    public string id;
    public string name;
    public string description;
    public int type;
    public string image;
    public int price;

    public ShopCatalogueItem()
    {
        name = "";
        description = "";
        type = 0;
        image = "";
        price = 0;
    }
}
