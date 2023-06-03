using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<PizzaRecipesSO> recipeList;
    public List<GameObject> emptyChairsToSit;
    [SerializeField]
    public List<OrderInfo> orderInfos;
    public float playerMoney;
    public float playerExperience;
    
}
