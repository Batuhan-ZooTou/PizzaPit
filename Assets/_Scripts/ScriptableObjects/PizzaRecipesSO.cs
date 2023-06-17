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
public enum DoughType
{
    slim,
    medium,
    thick,
}
[CreateAssetMenu(menuName ="ScriptableObjects/PizzaRecipes")]
public class PizzaRecipesSO : ScriptableObject
{
    public string pizzaName;
    public Sprite logo;
    public List<ItemSO> mustHave;
    
    public float cost;
    public float expEarn;

}
