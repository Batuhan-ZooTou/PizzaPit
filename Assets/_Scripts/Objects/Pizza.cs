using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Cooked
{
    notCooked,
    Cooked,
    OverCooked,
}
public class Pizza : MonoBehaviour
{
    public Cooked isCooked;
    public List<ItemSO> ingredients;
    public SauceType sauce;
    public DoughType dough;
    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    public LayerMask ingredientLayer;
    ObjectGrabbable addedIngredient;
    
    private void Update()
    {
        if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up,out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, ingredientLayer))
        {
            if (hit.collider.TryGetComponent(out addedIngredient))
            {
                if (addedIngredient.ItemSO!=null)
                {
                    ingredients.Add(addedIngredient.ItemSO);
                    Instantiate(addedIngredient.ItemSO.model, transform.position, Quaternion.Euler(Vector3.zero), transform);
                    addedIngredient.Drop();
                    hit.collider.gameObject.SetActive(false);
                    
                }
            }
        }
    }
    public void CheckForRecipe()
    {
        if (ingredients.Count==0)
        {
            Debug.Log("ReadyToServe");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents*2);
    }
}
