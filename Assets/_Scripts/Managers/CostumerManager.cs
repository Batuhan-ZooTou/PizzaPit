using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CostumerManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject prefab;
    public IObjectPool<Customer> costumerPool;
    public List<Customer> CostumersOnScene = new List<Customer>();
    public Transform costumerSpawnPoint;
    public Transform[] CostumerLinePoints;
    public int currentEmptyLine;
    // Start is called before the first frame update
    private void Awake()
    {
        costumerPool = new ObjectPool<Customer>(CreateCostumer, OnTakeFromPool, OnReturnedToPool, null, true, 10);
        StartCoroutine(Delay());
    }
    Customer CreateCostumer()
    {
        var customer = Instantiate(prefab,this.transform);
        return customer.GetComponent<Customer>();
    }
    void OnTakeFromPool(Customer customer)
    {
        customer.gameObject.SetActive(true);
        currentEmptyLine++;
        CostumersOnScene.Add(customer);
        customer.gameManager = gameManager;
        customer.costumerManager = this;
        customer.RandomizedOrder();
        customer.CalculateTotalCost();
        customer.transform.position = costumerSpawnPoint.position;
        customer.destination = CostumerLinePoints[currentEmptyLine-1];
        customer.currentLine = currentEmptyLine;
        customer.onLine = true;
    }
    public void OnReturnedToPool(Customer customer)
    {
        CostumersOnScene.Remove(customer);
        customer.gameManager = null;
        customer.onLine = false;
        customer.gameObject.SetActive(false);
        SpawnCostumer();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnCostumer()
    {
        costumerPool.Get();
    }
    IEnumerator Delay()
    {
        SpawnCostumer();
        yield return new WaitForSeconds(3);
        SpawnCostumer();
        yield return new WaitForSeconds(3);
        SpawnCostumer();
        yield return new WaitForSeconds(3);
        SpawnCostumer();
        yield return new WaitForSeconds(3);
        SpawnCostumer();
    }
    public void RealignCostumersOnLine(int lineNo)
    {
        foreach (var costumer in CostumersOnScene)
        {
            if (costumer.onLine)
            {
                if (costumer.currentLine > lineNo)
                {
                    costumer.currentLine--;
                    costumer.destination = CostumerLinePoints[costumer.currentLine - 1];
                    costumer.state = NpcState.WalkingToLine;
                    costumer.animator.SetBool("idle", false);

                }
            }
        }
    }
}
