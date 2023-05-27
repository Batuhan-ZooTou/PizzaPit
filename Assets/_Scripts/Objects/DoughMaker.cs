using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Pool;

public class DoughMaker : MonoBehaviour
{
    public DoughType Type;
    public PizzaSize Size;
    public bool isplaying;

    public GameObject PizzaPrefab;
    public Transform PlatePoint;

    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    public LayerMask doughLayer;
    ObjectGrabbable addedDough;

    public GameObject Smasher;
    private bool sizeWheelTurning;
    private bool typeWheelTurning;

    public IObjectPool<GameObject> pizzaPool;

    // Start is called before the first frame update
    void Awake()
    {
        pizzaPool = new ObjectPool<GameObject>(CreateIngredient, OnTakeFromPool, OnReturnedToPool, null, true, 10);

    }
    GameObject CreateIngredient()
    {
        var ingredient = Instantiate(PizzaPrefab, transform.position, transform.rotation, null);
        return ingredient;
    }
    void OnTakeFromPool(GameObject ingredient)
    {
        ingredient.gameObject.SetActive(true);
        ingredient.GetComponent<ObjectGrabbable>().ingredientPoll = pizzaPool;
    }
    void OnReturnedToPool(GameObject ingredient)
    {
        ingredient.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (!isplaying && addedDough==null)
        {
            if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, doughLayer))
            {
                if (hit.collider.TryGetComponent(out addedDough))
                {
                    if (addedDough.ItemSO == null)
                    {
                        addedDough = null;
                        return;
                    }
                    if (addedDough.ItemSO.type == BaseIngredient.dough)
                    {
                        addedDough.Drop();
                        addedDough.GetComponent<Interactable>().canInteract = false;
                        addedDough.objectRigidbody.velocity = Vector3.zero;
                        addedDough.gameObject.transform.position = PlatePoint.position;
                    }
                }
            }
        }
    }

    public void StartMachine(Transform valve)
    {
        if (isplaying || addedDough==null)
        {
            return;
        }
        isplaying = true;
        Smasher.transform.DOMoveY(PlatePoint.transform.position.y, 3).OnComplete(() => OnSmash());
        valve.DOBlendableLocalRotateBy(new Vector3(0, 180, 0), 3).OnComplete(() => valve.DOBlendableRotateBy(new Vector3(0, -180, 0), 3));
    }

    public void OnSmash()
    {
        Destroy(addedDough.gameObject);
        addedDough = null;
        //Pizza pizza = Instantiate(PizzaPrefab, PlatePoint.position, PlatePoint.rotation).GetComponent<Pizza>();
        Pizza pizza = pizzaPool.Get().GetComponent<Pizza>();
        pizza.transform.position = PlatePoint.position;
        pizza.transform.rotation = PlatePoint.rotation;
        //pizza.size = Size;
        //pizza.dough = Type;
        pizza.SetPizzaLook(Size, Type);
        Smasher.transform.DOMoveY(PlatePoint.transform.position.y + 0.4f, 3).OnComplete(()=> isplaying=false);
    }
    public void ChangeDoughType(Transform button)
    {
        if (typeWheelTurning || isplaying)
        {
            return;
        }
        typeWheelTurning = true;
        button.DOBlendableLocalRotateBy(new Vector3(0, -120, 0), 1).OnComplete(() => typeWheelTurning = false);
        switch (Type)
        {
            case DoughType.slim:
                Type = DoughType.medium;
                break;
            case DoughType.medium:
                Type = DoughType.thick;
                break;
            case DoughType.thick:
                Type = DoughType.slim;
                break;
            default:
                break;
        }
    }
    public void ChangePizzaSize(Transform button)
    {
        if (sizeWheelTurning || isplaying)
        {
            return;
        }
        sizeWheelTurning = true;
        button.DOBlendableLocalRotateBy(new Vector3(0, -120, 0), 1).OnComplete(() => sizeWheelTurning = false);
        switch (Size)
        {
            case PizzaSize.small:
                Size = PizzaSize.medium;
                break;
            case PizzaSize.medium:
                Size = PizzaSize.large;
                break;
            case PizzaSize.large:
                Size = PizzaSize.small;
                break;
            default:
                break;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents * 2);
    }
}
