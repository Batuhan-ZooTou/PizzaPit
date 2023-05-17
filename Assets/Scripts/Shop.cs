﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    public static Shop Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [System.Serializable]
    public class ShopItem
    {
        public Sprite itemImage;
        public int itemPrice;
        public bool IsPurchased = false;
        public int haveItem = 0;
        public string itemInfo;

    }

    public List<ShopItem> ShopItemsList;

    [SerializeField] Animator NoCoinsAnim;
    [SerializeField] GameObject ItemTemplate;
    public GameObject Item;
    [SerializeField] Transform ShopScrollView;
    [SerializeField] GameObject ShopPanel;
    Button buyButton;

    void Start()
    {
        int len = ShopItemsList.Count;
        for (int i = 0; i < len; i++)
        {
            Item = Instantiate(ItemTemplate, ShopScrollView);
            //Item.AddComponent<MouseOverInfo>();
            Item.transform.GetChild(0).GetComponent<Image>().sprite = ShopItemsList[i].itemImage;
            Item.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ShopItemsList[i].itemPrice.ToString();
            buyButton = Item.transform.GetChild(2).GetComponent<Button>();
            //Item.transform.GetChild(3).GetComponent<Text>().text = ShopItemsList[i].itemInfo;
            buyButton.AddEventListener(i, OnShopItemBtnClicked);
        }
    }
    
    void OnShopItemBtnClicked(int itemIndex)
    {
        if (Game.Instance.HasEnoughCoins(ShopItemsList[itemIndex].itemPrice))
        {
            Game.Instance.UseCoins(ShopItemsList[itemIndex].itemPrice);
            //purchase item
            ShopItemsList[itemIndex].IsPurchased = true;
            ShopItemsList[itemIndex].haveItem++;
            buyButton = ShopScrollView.GetChild(itemIndex).GetChild(2).GetComponent<Button>();
            Game.Instance.UpdateAllCoinsUIText();
        }
        else
        {
            NoCoinsAnim.SetTrigger("NoCoins");
            Debug.Log("you dont have coin");
        }
    }
}
