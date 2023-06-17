using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<PizzaRecipesSO> recipeList;
    public List<GameObject> emptyChairsToSit;
    [SerializeField]
    public List<OrderInfo> orderInfos;
    public float playerMoney;
    public float playerExperience;
    public GameObject OrderListUI;
    public GameObject RecipeListUI;
    public GameObject EscapeUI;
    public Slider expBar;
    public int Lvl=0;
    public TextMeshProUGUI level;
    public void UpdateOrderListUI(OrderInfo orderInfo)
    {
        OrderListUI.transform.GetChild(orderInfos.Count-1).gameObject.SetActive(true);
        OrderUI orderUI = OrderListUI.transform.GetChild(orderInfos.Count - 1).GetComponent<OrderUI>();
        orderUI.pizzaName.text = orderInfo.costumerName;
        orderUI.pizzaSprite.sprite = orderInfo.order.logo;
        orderUI.pizzaSize.text = orderInfo.pizzaSize.ToString();
        orderUI.pizzaDough.text = orderInfo.doughType.ToString();
        orderUI.cost.text = orderInfo.cost.ToString();
        for (int a = 0; a < orderInfo.extras.Count; a++)
        {
            orderUI.Extras.transform.GetChild(a).gameObject.SetActive(true);
            orderUI.Extras.transform.GetChild(a).GetComponent<TextMeshProUGUI>().text = orderInfo.extras[a].ingName;

        }
        return;
    }
    public void RemoveFromOrderListUI(OrderInfo orderInfo)
    {
        for (int i = 0; i < 8; i++)
        {
            OrderUI orderUI = OrderListUI.transform.GetChild(i).GetComponent<OrderUI>();
            if (orderUI.pizzaName.text==orderInfo.costumerName)
            {
                if (orderUI.cost.text == orderInfo.cost.ToString())
                {
                    OrderListUI.transform.GetChild(i).gameObject.SetActive(false);
                    orderInfos.Remove(orderInfo);
                    return;
                }
            }
        }
    }
    public void UpdateExpBar(float exp)
    {
        playerExperience += exp;
        expBar.value = playerExperience;
        if (playerExperience>expBar.maxValue)
        {
            Lvl++;
            level.text= Lvl.ToString();
            expBar.minValue = expBar.maxValue;
            expBar.maxValue *= 2;
        }
        else if(exp<0)
        {
            playerExperience = expBar.minValue;
        }
    }
    public void OpenOrderList()
    {
        if (OrderListUI.gameObject.activeInHierarchy)
        {
            OrderListUI.gameObject.SetActive(false);
        }
        else
        {
            OrderListUI.gameObject.SetActive(true);
        }
    }
    public void OpenRecipeList()
    {
        if (RecipeListUI.gameObject.activeInHierarchy)
        {
            RecipeListUI.gameObject.SetActive(false);
        }
        else
        {
            RecipeListUI.gameObject.SetActive(true);
        }
    }
    public void Escape()
    {
        if (EscapeUI.gameObject.activeInHierarchy)
        {
            Time.timeScale = 1;
            EscapeUI.gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            EscapeUI.gameObject.SetActive(true);
        }
    }
}
