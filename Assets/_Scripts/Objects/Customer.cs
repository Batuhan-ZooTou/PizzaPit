using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
public enum NpcState
{
    WalkingToLine,
    WalkingToChair,
    WaitingToOrder,
    WaitingForPizza,
    EatingPizza,
    LeavingStore,
}
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
    public NpcState state;
    private GameObject chair;
    private Outline outline;
    private Outline childOutline;

    public float eatTime;
    private float eatCounter;
    public float waitTime;
    private float waitCounter;
    public float pizzaWaitTime;
    private float pizzaWaitCounter;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        int rng = Random.Range(0,8);
        transform.GetChild(rng).gameObject.SetActive(true);
        outline = GetComponent<Outline>();
        childOutline = transform.GetChild(rng).gameObject.GetComponent<Outline>();
    }
    private void OnEnable()
    {
        hasOrder = true;
        state = NpcState.WalkingToLine;
        eatCounter = eatTime;
        waitCounter = waitTime;
        pizzaWaitCounter = pizzaWaitTime;
        //RandomizedOrder();
        //CalculateTotalCost();
    }
    private void Update()
    {
        FixOutline();
        switch (state)
        {
            case NpcState.WalkingToLine:
                navMeshAgent.destination = destination.position;
                if (Vector3.Distance(transform.position, destination.position)>= 0.2f)
                {
                    animator.SetBool("walk",true);
                }
                else
                {
                    animator.SetBool("walk",false);
                    state = NpcState.WaitingToOrder;
                }
                break;
            case NpcState.WalkingToChair:
            navMeshAgent.destination = destination.position;
                if (Vector3.Distance(transform.position, destination.position) >= 0.4f)
                {
                    animator.SetBool("walk",true);
                }
                else
                {
                    GetComponent<Collider>().isTrigger = true;
                    transform.position = destination.position;
                    transform.rotation = destination.rotation;
                    animator.SetBool("walk",false);
                    state = NpcState.WaitingForPizza;
                }
                break;
            case NpcState.WaitingToOrder:
                animator.SetBool("idle", true);
                isStopped = true;
                onLine = true;
                hasOrder = true;
                if (currentLine==1)
                {
                    waitCounter -= Time.deltaTime;
                    if (waitCounter <= 0)
                    {
                        destination = costumerManager.costumerSpawnPoint;
                        costumerManager.currentEmptyLine--;
                        animator.SetBool("idle", false);
                        costumerManager.RealignCostumersOnLine(currentLine);
                        state = NpcState.LeavingStore;
                    }
                }
                break;
            case NpcState.WaitingForPizza:
                animator.SetBool("sit", true);
                pizzaWaitCounter -= Time.deltaTime;
                if (navMeshAgent.baseOffset>-0.1)
                {
                    navMeshAgent.baseOffset -= Time.deltaTime;
                }
                else
                {
                    navMeshAgent.baseOffset = -0.11f;
                }
                if (pizzaWaitCounter<=0)
                {
                    destination = costumerManager.costumerSpawnPoint;
                    pizzaWaitCounter = pizzaWaitTime;
                    animator.SetBool("sit", false);
                    state = NpcState.LeavingStore;
                }
                
                break;
            case NpcState.EatingPizza:
                eatCounter -= Time.deltaTime;
                if (eatCounter<=0)
                {
                    animator.SetBool("sit", false);
                    eatCounter = eatTime;
                    destination = costumerManager.costumerSpawnPoint;
                    costumerManager.gameManager.emptyChairsToSit.Add(chair);
                    state = NpcState.LeavingStore;
                }
                break;
            case NpcState.LeavingStore:
                navMeshAgent.destination = destination.position;
                animator.SetBool("walk", true);
                navMeshAgent.baseOffset = 0;
                if (Vector3.Distance(transform.position, destination.position) <= 0.25f)
                {
                    costumerManager.OnReturnedToPool(this);
                }

                break;
            default:
                break;
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
        if (currentLine==1 && hasOrder)
        {
            state = NpcState.WalkingToChair;
            onLine = false;
            hasOrder = false;
            isStopped = false;
            costumerManager.currentEmptyLine--;
            destination = costumerManager.gameManager.emptyChairsToSit[0].transform;
            chair = costumerManager.gameManager.emptyChairsToSit[0];
            costumerManager.gameManager.emptyChairsToSit.RemoveAt(0);
            animator.SetBool("idle", false);

            costumerManager.RealignCostumersOnLine(currentLine);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out pizza))
        {
            TakePizza();
            state = NpcState.EatingPizza;
        }
    }
    public void FixOutline()
    {
        childOutline.OutlineWidth = outline.OutlineWidth;
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
