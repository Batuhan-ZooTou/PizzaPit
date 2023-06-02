using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum Cooked
{
    notCooked,
    Cooked,
    OverCooked,
}
public enum PizzaSize
{
    small,
    medium,
    large,
}
public class Pizza : MonoBehaviour
{
    public Cooked isCooked;
    public List<ItemSO> ingredients;
    public bool isSauced;
    public bool insideBox;
    public PizzaSize size;
    public DoughType dough;
    [SerializeField] Vector3 colliderExtents;
    [SerializeField] float maxDisstance;
    public LayerMask ingredientLayer;
    ObjectGrabbable addedIngredient;
    public Material cookedMat;
    public Material overCookedMat;
    public GameObject IngredientUI;
    public List<ItemSO> ingredientsTypes;
    public List<ItemSO> ingredientCounts;

    private void Update()
    {
        if (Physics.BoxCast(transform.position, colliderExtents, Vector3.up,out RaycastHit hit, Quaternion.Euler(Vector3.zero), maxDisstance, ingredientLayer))
        {
            if (hit.collider.TryGetComponent(out addedIngredient))
            {
                if (addedIngredient.ItemSO==null || insideBox)
                {
                    return;
                }
                if (addedIngredient.ItemSO.type==BaseIngredient.none)
                {
                    ingredients.Add(addedIngredient.ItemSO);
                    Instantiate(addedIngredient.ItemSO.model, transform.position, Quaternion.Euler(Vector3.zero), transform);
                    addedIngredient.Drop();
                    OnIngredientAddedUpdateUI();
                    hit.collider.gameObject.SetActive(false);
                    
                }
            }
        }
    }
    void OnIngredientAddedUpdateUI()
    {
        if (ingredients==null)
        {
            ingredientsTypes.Add(addedIngredient.ItemSO);
            GameObject temp = IngredientUI.transform.GetChild(0).gameObject;
            temp.SetActive(true);
            temp.GetComponent<Image>().sprite = addedIngredient.ItemSO.itemIcon;
            temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
            return;
        }
        if (ingredientsTypes.Contains(addedIngredient.ItemSO))
        {
            for (int i = 0; i < ingredientsTypes.Count; i++)
            {
                if (addedIngredient.ItemSO== ingredientsTypes[i])
                {
                    ingredientCounts.Add(addedIngredient.ItemSO);
                    IngredientUI.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetCount().ToString();
                    return;
                }
            }
        }
        else
        {
            ingredientsTypes.Add(addedIngredient.ItemSO);
            GameObject temp = IngredientUI.transform.GetChild(ingredientsTypes.Count - 1).gameObject;
            temp.SetActive(true);
            temp.GetComponent<Image>().sprite = addedIngredient.ItemSO.itemIcon;
            temp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "1";
        }
    }
    int GetCount()
    {
        int count=1;
        foreach (var item in ingredientCounts)
        {
            if (addedIngredient.ItemSO==item)
            {
                count++;
            }
        }
        return count;
    }
    public void SetPizzaLook(PizzaSize _size,DoughType _type)
    {
        size = _size;
        dough = _type;
        switch (size)
        {
            case PizzaSize.small:
                transform.localScale = new Vector3(1.3f,transform.localScale.y,1.3f);
                break;
            case PizzaSize.medium:
                transform.localScale = new Vector3(1.6f,transform.localScale.y,1.6f);
                break;
            case PizzaSize.large:
                transform.localScale = new Vector3(1.9f,transform.localScale.y,1.9f);
                break;
            default:
                break;
        }
        switch (dough)
        {
            case DoughType.slim:
                transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
                break;
            case DoughType.medium:
                transform.localScale = new Vector3(transform.localScale.x, 1.5f, transform.localScale.z);
                break;
            case DoughType.thick:
                transform.localScale = new Vector3(transform.localScale.x, 2f, transform.localScale.z);
                break;
            default:
                break;
        }
    }
    public void CookPizza()
    {
        switch (isCooked)
        {
            case Cooked.notCooked:
                break;
            case Cooked.Cooked:
                GetComponent<MeshRenderer>().material = cookedMat;
                break;
            case Cooked.OverCooked:
                GetComponent<MeshRenderer>().material = overCookedMat;
                break;
            default:
                break;
        }
    }
    public void CheckForRecipe()
    {
        if (ingredients.Count==0)
        {
            Debug.Log("ReadyToServe");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sauce"))
        {
            isSauced = true;
            transform.GetChild(0).gameObject.SetActive(true);
            Destroy(collision.gameObject);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = transparentRed;
        Gizmos.DrawCube(transform.position + Vector3.up * maxDisstance, colliderExtents*2);
    }
}
