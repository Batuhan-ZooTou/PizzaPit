using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public void UpdateOrderListUI(OrderInfo orderInfo)
    {
        OrderListUI.transform.GetChild(orderInfos.Count-1).gameObject.SetActive(true);
        OrderUI orderUI = OrderListUI.transform.GetChild(orderInfos.Count - 1).GetComponent<OrderUI>();
        orderUI.pizzaName.text = orderInfo.costumerName;
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
}
