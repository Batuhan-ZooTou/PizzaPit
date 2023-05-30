using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;
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
    [Header("Order")]
    public Pizza pizza;
    public PizzaRecipesSO order;
    public List<ItemSO> extras =new List<ItemSO>();
    public PizzaSize pizzaSize;
    public DoughType doughType;
    public float cost;
    public int maxExtraCount;
    public int pleased;
    //refs
    [HideInInspector]public GameManager gameManager;
    [HideInInspector] public CostumerManager costumerManager;
    [HideInInspector]public Animator animator;
    private NavMeshAgent navMeshAgent;
    private GameObject chair;
    private Outline outline;
    private Outline childOutline;

    [Header("Informations")]
    public GameObject imageAbove;
    public Transform pizzaHoldPoint;
    public Transform destination;
    public int currentLine;
    public bool onLine;
    public bool hasOrder;
    public NpcState state;

    [Header("Timers")]
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
        GetComponent<Interactable>().canInteract = true;
        pleased = 0;
        state = NpcState.WalkingToLine;
        eatCounter = eatTime;
        waitCounter = waitTime;
        pizzaWaitCounter = pizzaWaitTime;
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
                    pizza.GetComponent<ObjectGrabbable>().ingredientPoll.Release(pizza.gameObject);
                    destination = costumerManager.costumerSpawnPoint;
                    costumerManager.gameManager.emptyChairsToSit.Add(chair);
                    state = NpcState.LeavingStore;
                    imageAbove.transform.DOMoveY(imageAbove.transform.position.y - 0.1f, 1).SetEase(Ease.OutQuad).OnComplete(()=> imageAbove.SetActive(false));

                }
                break;
            case NpcState.LeavingStore:
                navMeshAgent.destination = destination.position;
                animator.SetBool("walk", true);
                navMeshAgent.baseOffset = 0;
                pizza = null;
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
        bool perfect=true;
        //if order missing ingredient
        IEnumerable<ItemSO> missingIngredients = order.mustHave.Except(pizza.ingredients);
        if (missingIngredients != null)
        {
            foreach (var item in missingIngredients)
            {
                Debug.Log("customer  wants " + item);
                pleased--;
                perfect = false;
            }
        }
        //removes main
        foreach (var item in order.mustHave)
        {
            //removes extras
            if (pizza.ingredients.Contains(item))
            {
                pizza.ingredients.Remove(item);
                pleased++;
            }
        }
        //if order missing ingredient
        IEnumerable<ItemSO> missingExtras = extras.Except(pizza.ingredients);
        if (missingExtras != null)
        {
            foreach (var item in missingExtras)
            {
                Debug.Log("customer  wants " + item);
                pleased--;
                perfect = false;
            }
        }
        //check for extras
        foreach (var item in extras)
        {
            //removes extras
            if (pizza.ingredients.Contains(item))
            {
                pizza.ingredients.Remove(item);
                pleased++;
            }
        }
        //if order has unwanted ingredient
        IEnumerable<ItemSO> unwantedIngredients = pizza.ingredients.Except(order.mustHave);
        if (unwantedIngredients != null)
        {
            foreach (var item in unwantedIngredients)
            {
                Debug.Log("customer doesnt want " + item);
                pleased--;
                perfect = false;
            }
        }
        //if order has unwanted ingredient
        IEnumerable<ItemSO> unwantedExtras = pizza.ingredients.Except(extras);
        if (unwantedExtras != null)
        {
            foreach (var item in unwantedExtras)
            {
                Debug.Log("customer doesnt want " + item);
                pleased--;
                perfect = false;
            }
        }
        //all good
        if (perfect)
        {
            Debug.Log("yep all correct");
            pleased++;
        }
        //more than ordered extra
        else
        {
            foreach (var item in pizza.ingredients)
            {
                Debug.Log("extra" + item);
                pleased--;
            }
        }
        //if (pizza.ingredients.ToHashSet().SetEquals(order.mustHave))
        //{
        //    //removes main
        //    foreach (var item in order.mustHave)
        //    {
        //        pizza.ingredients.Remove(item);
        //        pleased++;
        //    }
        //    //check for extras
        //    if (pizza.ingredients.ToHashSet().SetEquals(extras))
        //    {
        //        foreach (var item in extras)
        //        {
        //            
        //            //removes extras
        //            if (pizza.ingredients.Contains(item))
        //            {
        //                pizza.ingredients.Remove(item);
        //                pleased++;
        //            }
        //            //missing extras count wise
        //            else
        //            {
        //                Debug.Log("not enough" + item);
        //                pleased--;
        //                return;
        //            }
        //        }
        //        //all good
        //        if (pizza.ingredients.Count==0)
        //        {
        //            Debug.Log("yep all correct");
        //            pleased++;
        //        }
        //        //more than ordered extra
        //        else
        //        {
        //            foreach (var item in pizza.ingredients)
        //            {
        //                Debug.Log("extra" + item);
        //                pleased--;
        //            }
        //        }
        //    }
        //    //missing extras type wise
        //    else
        //    {
        //        Debug.Log("extra nope");
        //    }
        //}
        //else
        //{
        //    //if order missing ingredient
        //    IEnumerable<ItemSO> missingIngredients = order.mustHave.Except(pizza.ingredients);
        //    if (missingIngredients!=null)
        //    {
        //        foreach (var item in missingIngredients)
        //        {
        //            Debug.Log("customer  wants " + item);
        //            pleased--;
        //        }
        //    }
        //    //if order missing ingredient
        //    IEnumerable<ItemSO> missingExtras = extras.Except(pizza.ingredients);
        //    if (missingExtras != null)
        //    {
        //        foreach (var item in missingExtras)
        //        {
        //            Debug.Log("customer  wants " + item);
        //            pleased--;
        //        }
        //    }
        //    //if order has unwanted ingredient
        //    IEnumerable<ItemSO> unwantedIngredients = pizza.ingredients.Except(order.mustHave);
        //    if(unwantedIngredients!=null)
        //    {
        //        foreach (var item in unwantedIngredients)
        //        {
        //            Debug.Log("customer doesnt want "+item);
        //            pleased--;
        //        }
        //    }
        //    
        //}
        if (pizza.dough!=doughType)
        {
            Debug.Log("Costumer wants" +doughType);
            pleased--;
        }
        else
        {
            Debug.Log("Correct type");
            pleased++;
        }
        if (pizza.size!=pizzaSize)
        {
            Debug.Log("Costumer wants" + pizzaSize);
            pleased--;
        }
        else
        {
            Debug.Log("Correct size");
            pleased++;
        }
        if (!pizza.isSauced)
        {
            pleased--;
        }
        else
        {
            pleased++;
        }
        if (pizza.isCooked!=Cooked.Cooked)
        {
            pleased--;
        }
        else
        {
            pleased++;
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
        if (currentLine==1 && hasOrder && state==NpcState.WaitingToOrder)
        {
            GetComponent<Interactable>().canInteract = false;
            state = NpcState.WalkingToChair;
            onLine = false;
            hasOrder = false;
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
        if (other.GetComponent<Doors>()!=null)
        {
            other.GetComponent<Doors>().Open();
        }
        if (state!=NpcState.WaitingForPizza)
        {
            return;
        }
        if (other.TryGetComponent(out pizza))
        {
            TakePizza();
            gameManager.playerExperience += order.expEarn * pleased;
            pizza.GetComponent<ObjectGrabbable>().objectRigidbody.velocity = Vector3.zero;
            pizza.GetComponent<ObjectGrabbable>().Drop();
            pizza.transform.position = chair.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.position;
            if (pleased<0)
            {
                gameManager.playerMoney += 0;
                animator.SetBool("sit", false);
                destination = costumerManager.costumerSpawnPoint;
                costumerManager.gameManager.emptyChairsToSit.Add(chair);
                state = NpcState.LeavingStore;
            }
            else
            {
                gameManager.playerMoney += cost;
                imageAbove.SetActive(true);
                imageAbove.transform.DOMoveY(imageAbove.transform.position.y+0.1f,1).SetEase(Ease.OutQuad);
                state = NpcState.EatingPizza;
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Doors>() != null)
        {
            other.GetComponent<Doors>().Open();
        }
    }
    public void FixOutline()
    {
        childOutline.OutlineWidth = outline.OutlineWidth;
    }
    private void GetRandomRecipe()
    {
        int rng = Random.Range(0, gameManager.recipeList.Count);
        order=gameManager.recipeList[rng];
    }
    private void GetRandomExtras()
    {
        int extraCount = Random.Range(0, maxExtraCount+1);
        for (int i = 0; i < extraCount; i++)
        {
            int extraType = Random.Range(0, order.mustHave.Count);
            extras.Add(order.mustHave[extraType]);

        }
    }
    private void GetRandomPizzaSize()
    {
        int rng = Random.Range(0, 3);
        pizzaSize = (PizzaSize)rng;
    }
    private void GetRandomDoughType()
    {
        int rng = Random.Range(0, 3);
        doughType = (DoughType)rng;
    }
    public void GetRandomOrder()
    {
        RandomizedOrder();
        CalculateTotalCost();
        GetRandomPizzaSize();
        GetRandomDoughType();
    }
}
