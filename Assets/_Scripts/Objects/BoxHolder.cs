using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BoxHolder : MonoBehaviour
{
    public GameObject prefab;
    public IObjectPool<GameObject> boxPoll;
    public int boxCount;
    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    public LayerMask boxLayer;
    PizzaBox box;
    public float timeToTakeBackIngredient;
    public bool canReturnToPool = true;
    BoxCollider boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        boxPoll = new ObjectPool<GameObject>(CreateIngredient, OnTakeFromPool, OnReturnedToPool, null, true, 10);
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y+(0.3f*boxCount), boxCollider.size.z);
        maxDisstance += 0.1f * boxCount;
        for (int i = 0; i < boxCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    private void FixedUpdate()
    {
        if (canReturnToPool && boxCount<10)
        {
            if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, boxLayer))
            {
                if (hit.collider.TryGetComponent(out box))
                {
                    box.GetComponent<ObjectGrabbable>().Drop();
                    boxPoll.Release(box.gameObject);
                }
            }
        }
    }
    GameObject CreateIngredient()
    {
        var ingredient = Instantiate(prefab, transform.position, transform.rotation, null);
        return ingredient;
    }
    void OnTakeFromPool(GameObject ingredient)
    {
        ingredient.gameObject.SetActive(true);
        ingredient.GetComponent<ObjectGrabbable>().ingredientPoll = boxPoll;
    }
    public void OnInteract()
    {
        if (boxCount>0)
        {
            transform.GetChild(boxCount-1).gameObject.SetActive(false);
            canReturnToPool = false;
            boxCount--;
            maxDisstance -= 0.1f;
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y-0.3f, boxCollider.size.z);
            Invoke("ResetReturnPool", timeToTakeBackIngredient);
        }
        
    }
    void ResetReturnPool()
    {
        if (!canReturnToPool)
        {
                canReturnToPool = true;
        }
    }
    void OnReturnedToPool(GameObject ingredient)
    {
        maxDisstance += 0.1f;
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y+0.3f, boxCollider.size.z);
        boxCount++;
        transform.GetChild(boxCount-1).gameObject.SetActive(true);
        ingredient.gameObject.SetActive(false);
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents * 2);
    }
}
