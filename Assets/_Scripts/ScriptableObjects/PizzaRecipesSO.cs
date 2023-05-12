using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Ingredients
{
    Cheese,
    pepper,
    tomato,
    mushroom,
    olive,
    sausage,
    corn,
}
public enum SauceType
{
    tomato,

}
public enum DoughType
{
    slim,
    medium,
    fat,
    any
}
[CreateAssetMenu(menuName ="ScriptableObjects/PizzaRecipes")]
public class PizzaRecipesSO : ScriptableObject
{
    public string pizzaName;
    public GameObject prefab;
    public List<ItemSO> mustHave;
    
    public float cost;
    public float expEarn;

}
