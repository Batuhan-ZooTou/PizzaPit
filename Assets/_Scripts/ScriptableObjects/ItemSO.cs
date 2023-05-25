using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PizzaIngredients")]
public class ItemSO : ScriptableObject
{
    public int cost;
    public GameObject model;
    public GameObject Prefab;
    public Sprite itemIcon;
    public BaseIngredient type;
}
public enum BaseIngredient
{
    dough,
    sauce,
    none,
}
