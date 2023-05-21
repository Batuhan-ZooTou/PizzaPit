using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game Instance;
    public GameManager gm;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] Text[] allCoinsUIText;

    //public int Coins;

    void Start()
    {
        UpdateAllCoinsUIText();
    }

    public void UseCoins(int amount)
    {
        gm.playerMoney -= amount;
    }

    public bool HasEnoughCoins(int amount)
    {
        return (gm.playerMoney >= amount);
    }

    public void UpdateAllCoinsUIText()
    {
        for (int i = 0; i < allCoinsUIText.Length; i++)
        {
            allCoinsUIText[i].text = gm.playerMoney.ToString();
        }
    }

}
