using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PizzaIngredients")]
public class ItemSO : ScriptableObject
{
    public float cost;
    public GameObject model;
}
