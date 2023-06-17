using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Phone : MonoBehaviour
{
    [SerializeField]private DeliveryGuy deliveryGuy;
    [SerializeField]private GameManager gameManager;
    [SerializeField] private int maxExtraCount;
    [SerializeField] public OrderInfo orderInfo;
    public float maxTime, minTime;
    private PizzaRecipesSO order;
    private List<ItemSO> extras = new List<ItemSO>();
    private PizzaSize pizzaSize;
    private DoughType doughType;
    private float cost;
    private bool isRinging;
    string[] NpcNames;
    public TextAsset MixList;
    private void Start()
    {
        NpcNames = MixList.text.Split("\n");
        Invoke("RingPhone", Random.Range(minTime,maxTime));
    }
    public void OnInteract()
    {
        if (isRinging)
        {
            isRinging = false;
            orderInfo = new OrderInfo(GetRandomRecipe(), GetRandomExtras(), GetRandomPizzaSize(), GetRandomDoughType(), CalculateTotalCost(), "Delivery");
            gameManager.orderInfos.Add(orderInfo);
            gameManager.UpdateOrderListUI(orderInfo);
            //NpcNames[Random.Range(0, NpcNames.Length)]
            deliveryGuy.orderInfos=orderInfo;
            deliveryGuy.GetComponent<Interactable>().canInteract = true;
            GetComponent<Interactable>().canInteract = false;
            Invoke("RingPhone", Random.Range(minTime, maxTime));
        }
    }
    
    public void RingPhone()
    {
        isRinging = true;
        GetComponent<Interactable>().canInteract = true;
    }
    public float CalculateTotalCost()
    {
        foreach (var item in extras)
        {
            cost += item.cost;
        }
        cost += order.cost;
        return cost;
    }
    private PizzaRecipesSO GetRandomRecipe()
    {
        int rng = Random.Range(0, gameManager.recipeList.Count);
        order = gameManager.recipeList[rng];
        return order;
    }
    private List<ItemSO> GetRandomExtras()
    {
        int extraCount = Random.Range(0, maxExtraCount + 1);
        for (int i = 0; i < extraCount; i++)
        {
            int extraType = Random.Range(0, order.mustHave.Count);
            extras.Add(order.mustHave[extraType]);

        }
        return extras;
    }
    private PizzaSize GetRandomPizzaSize()
    {
        int rng = Random.Range(0, 3);
        pizzaSize = (PizzaSize)rng;
        return pizzaSize;
    }
    private DoughType GetRandomDoughType()
    {
        int rng = Random.Range(0, 3);
        doughType = (DoughType)rng;
        return doughType;
    }
}
