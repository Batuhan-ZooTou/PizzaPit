using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class IngredientTins : MonoBehaviour
{
    public GameObject prefab;
    public ItemSO item;
    public float ingredientCount;
    public IObjectPool<GameObject> ingredientPoll;
    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    public LayerMask ingredientLayer;
    ObjectGrabbable addedIngredient;
    public float timeToTakeBackIngredient;
    private float counterToReset;
    public bool canReturnToPool=true;
    private void Awake()
    {
        ingredientPoll = new ObjectPool<GameObject>(CreateIngredient, OnTakeFromPool, OnReturnedToPool, null, true, 10);
        counterToReset = timeToTakeBackIngredient;
    }
    private void FixedUpdate()
    {
        if (canReturnToPool)
        {
            if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, ingredientLayer))
            {
                if (hit.collider.TryGetComponent(out addedIngredient))
                {
                    if (addedIngredient.ItemSO == item)
                    {
                        addedIngredient.Drop();
                        ingredientPoll.Release(addedIngredient.gameObject);
                    }
                }
            }
        }
    }
    private void Update()
    {
        ResetReturnPool();
    }
    GameObject CreateIngredient()
    {
        var ingredient = Instantiate(prefab,transform.position,transform.rotation,this.transform);
        return ingredient;
    }
    void OnTakeFromPool(GameObject ingredient)
    {
        ingredient.gameObject.SetActive(true);
    }
    void OnReturnedToPool(GameObject ingredient)
    {
        ingredient.gameObject.SetActive(false);
        ingredientCount++;
    }
    public void OnInteract()
    {
        if (ingredientCount>0)
        {
            canReturnToPool = false;
            ingredientCount--;
            counterToReset = timeToTakeBackIngredient;
        }
    }
    void ResetReturnPool()
    {
        if (!canReturnToPool)
        {
            counterToReset -= Time.deltaTime;
            if (counterToReset <= 0)
            {
                counterToReset = timeToTakeBackIngredient;
                canReturnToPool = true;
            }
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents * 2);
    }
}
