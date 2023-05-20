using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
public class Customer : MonoBehaviour
{
    public PizzaRecipesSO order;
    public GameManager gameManager;
    public CostumerManager costumerManager;
    public Pizza pizza;
    public List<ItemSO> extras =new List<ItemSO>();
    public float cost;
    public int maxExtraCount;
    private NavMeshAgent navMeshAgent;
    public Transform pizzaHoldPoint;
    public int currentLine;
    public bool onLine;
    public Transform destination;
    [HideInInspector]public Animator animator;
    public bool hasOrder;
    private bool isStopped;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        int rng = Random.Range(0,8);
        transform.GetChild(rng).gameObject.SetActive(true);

    }
    private void OnEnable()
    {
        hasOrder = true;
        //RandomizedOrder();
        //CalculateTotalCost();
    }
    private void Update()
    {
        navMeshAgent.destination = destination.position;
        if (Vector3.Distance(transform.position,destination.position)<=0.2f && !isStopped)
        {
            isStopped = true;
            animator.SetTrigger("idle");
        }
    }
    public void TakePizza()
    {
        if (pizza.ingredients.ToHashSet().SetEquals(order.mustHave))
        {
            //removes main
            foreach (var item in order.mustHave)
            {
                pizza.ingredients.Remove(item);
            }
            //check for extras
            if (pizza.ingredients.ToHashSet().SetEquals(extras))
            {
                foreach (var item in extras)
                {
                    
                    //removes extras
                    if (pizza.ingredients.Contains(item))
                    {
                        pizza.ingredients.Remove(item);
                    }
                    //missing extras count wise
                    else
                    {
                        Debug.Log("not enough" + item);
                        return;
                    }
                }
                //all good
                if (pizza.ingredients.Count==0)
                {
                    Debug.Log("yep all correct");
                    gameManager.playerExperience += order.expEarn;
                    gameManager.playerMoney += cost;
                    pizza.GetComponent<ObjectGrabbable>().CostumerHold(pizzaHoldPoint);

                }
                //more than ordered extra
                else
                {
                    foreach (var item in pizza.ingredients)
                    {
                        Debug.Log("extra" + item);

                    }
                }
            }
            //missing extras type wise
            else
            {
                Debug.Log("extra nope");
            }
        }
        else
        {
            //if order missing ingredient
            IEnumerable<ItemSO> missingIngredients = order.mustHave.Except(pizza.ingredients);
            if (missingIngredients!=null)
            {
                foreach (var item in missingIngredients)
                {
                    Debug.Log("customer  wants " + item);

                }
            }
            //if order has unwanted ingredient
            IEnumerable<ItemSO> unwantedIngredients = pizza.ingredients.Except(order.mustHave);
            if(unwantedIngredients!=null)
            {
                foreach (var item in unwantedIngredients)
                {
                    Debug.Log("customer doesnt want "+item);
                }
            }
            
        }
        
    }
    public void CalculateTotalCost()
    {
        foreach (var item in extras)
        {
            cost += item.cost;
        }
        cost +=order.cost;
    }
    public void RandomizedOrder()
    {
        GetRandomRecipe();
        GetRandomExtras();
    }
    public void OnInteract()
    {
        onLine = false;
        hasOrder = false;
        isStopped = false;
        costumerManager.currentEmptyLine--;
        animator.SetTrigger("walk");
        destination = costumerManager.costumerSpawnPoint;

        costumerManager.RealignCostumersOnLine(currentLine);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out pizza))
        {
            TakePizza();
            onLine = false;
            OnInteract();
            destination = costumerManager.costumerSpawnPoint;
        }
    }
    public void GetRandomRecipe()
    {
        int rng = Random.Range(0, gameManager.recipeList.Count);
        order=gameManager.recipeList[rng];
    }
    public void GetRandomExtras()
    {
        int extraCount = Random.Range(0, maxExtraCount+1);
        for (int i = 0; i < extraCount; i++)
        {
            int extraType = Random.Range(0, order.mustHave.Count);
            extras.Add(order.mustHave[extraType]);

        }
    }
}
