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
    string[] FemaleNames;
    string[] MaleNames;
    public TextAsset MaleList;
    public TextAsset FemaleList;
    // Start is called before the first frame update
    private void Awake()
    {
        FemaleNames = FemaleList.text.Split("\n");
        MaleNames = MaleList.text.Split("\n");
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
        int rng = Random.Range(0, 7);
        if (rng<4)
        {
            customer.NameTag.text = MaleNames[Random.Range(0, MaleNames.Length)];
        }
        else
        {
            customer.NameTag.text = FemaleNames[Random.Range(0, FemaleNames.Length)];

        }
        customer.transform.GetChild(rng).gameObject.SetActive(true);
        currentEmptyLine++;
        CostumersOnScene.Add(customer);
        customer.gameManager = gameManager;
        customer.costumerManager = this;
        customer.GetRandomOrder();
        customer.transform.position = costumerSpawnPoint.position;
        customer.destination = CostumerLinePoints[currentEmptyLine-1];
        customer.currentLine = currentEmptyLine;
        customer.onLine = true;

    }
    public void OnReturnedToPool(Customer customer)
    {
        gameManager.orderInfos.Remove(customer.orderInfo);
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
        yield return new WaitForSeconds(15);
        SpawnCostumer();
        yield return new WaitForSeconds(30);
        SpawnCostumer();
        yield return new WaitForSeconds(60);
        SpawnCostumer();
        yield return new WaitForSeconds(120);
        SpawnCostumer();
        yield return new WaitForSeconds(240);
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
