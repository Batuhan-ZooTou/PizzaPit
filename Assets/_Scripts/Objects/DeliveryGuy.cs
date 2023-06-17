using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class DeliveryGuy : MonoBehaviour
{
    [SerializeField]private GameManager gameManager;
    [SerializeField] public OrderInfo orderInfos;
    [SerializeField] public Pizza pizza;
    [HideInInspector]public Animator animator;
    private NavMeshAgent navMeshAgent;
    public float maxTime, minTime;
    public int pleased;
    public Transform outside;
    private Vector3 sp;
    bool delivering;
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        sp = transform.position;
        navMeshAgent.destination = sp;
        animator = GetComponent<Animator>(); 
        animator.SetBool("idle", true);
        animator.SetBool("walk", false);
    }
    public void OnInteract()
    {
        if (pizza!=null && orderInfos.costumerName!=null)
        {
            GetComponent<Interactable>().canInteract = false;
            Invoke("DeliverPizza", Random.Range(minTime, maxTime));
            animator.SetBool("idle", false);
            animator.SetBool("walk",true);
            navMeshAgent.destination = outside.position;
        }
        
    }
    private void Update()
    {
        if (delivering)
        {
            if (Vector3.Distance(transform.position, sp) <= 0.2f)
            {
                animator.SetBool("idle", true);
                animator.SetBool("walk", false);
                delivering = false;
            }
        }
    }
    public void DeliverPizza()
    {
        bool perfect = true;
        //if order missing ingredient
        IEnumerable<ItemSO> missingIngredients = orderInfos.order.mustHave.Except(pizza.ingredients);
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
        foreach (var item in orderInfos.order.mustHave)
        {
            //removes extras
            if (pizza.ingredients.Contains(item))
            {
                pizza.ingredients.Remove(item);
                pleased++;
            }
        }
        //if order missing ingredient
        IEnumerable<ItemSO> missingExtras = orderInfos.extras.Except(pizza.ingredients);
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
        foreach (var item in orderInfos.extras)
        {
            //removes extras
            if (pizza.ingredients.Contains(item))
            {
                pizza.ingredients.Remove(item);
                pleased++;
            }
        }
        //if order has unwanted ingredient
        IEnumerable<ItemSO> unwantedIngredients = pizza.ingredients.Except(orderInfos.order.mustHave);
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
        IEnumerable<ItemSO> unwantedExtras = pizza.ingredients.Except(orderInfos.extras);
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
        if (pizza.dough != orderInfos.doughType)
        {
            Debug.Log("Costumer wants" + orderInfos.doughType);
            pleased--;
        }
        else
        {
            Debug.Log("Correct type");
            pleased++;
        }
        if (pizza.size != orderInfos.pizzaSize)
        {
            Debug.Log("Costumer wants" + orderInfos.pizzaSize);
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
        if (pizza.isCooked != Cooked.Cooked)
        {
            pleased--;
        }
        else
        {
            pleased++;
        }
        gameManager.RemoveFromOrderListUI(orderInfos);
        gameManager.UpdateExpBar(orderInfos.order.expEarn * pleased);
        if (pleased>=0)
        {
            gameManager.playerMoney += orderInfos.cost;
        }
        pleased = 0;
        pizza = null;
        orderInfos = null;
        navMeshAgent.destination = sp; 
        delivering = true;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Doors>() != null)
        {
            other.GetComponent<Doors>().Open();
        }
        if (orderInfos==null)
        {
            return;
        }
        if (other.TryGetComponent<PizzaBox>(out PizzaBox pizzaBox))
        {
            if (pizzaBox.myPizza!=null)
            {
                pizza=pizzaBox.myPizza;
                pizza.GetComponent<ObjectGrabbable>().Drop();
                other.gameObject.SetActive(false);
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
}
